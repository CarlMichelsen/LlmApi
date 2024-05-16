using System.Runtime.CompilerServices;
using System.Text;
using Domain.Abstraction;
using Domain.Entity.Id;
using Interface.Repository;
using Interface.Service;
using LargeLanguageModelClient;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Microsoft.Extensions.Logging;

namespace Implementation.Service;

public class TrackedLargeLanguageModelService(
    ILogger<LargeLanguageModelService> logger,
    LargeLanguageModelService largeLanguageModelService,
    ILlmModelRepository llmModelRepository,
    IPromptRepository promptRepository) : ILargeLanguageModelService
{
    public async Task<Result<LlmResponse>> Prompt(
        LlmPromptDto llmPromptDto,
        CancellationToken cancellationToken)
    {
        var modelResult = await llmModelRepository
            .GetModel(new ModelEntityId(llmPromptDto.ModelIdentifier));
        if (modelResult.IsError)
        {
            return modelResult.Error!;
        }

        var startTime = DateTime.UtcNow;
        var clientResult = await largeLanguageModelService.Prompt(llmPromptDto, modelResult.Unwrap(), cancellationToken);
        if (clientResult.IsSuccess)
        {
            var response = clientResult.Unwrap();
            var content = response.Message.Content.Select(x => (ContentType: x.Type, Content: x.GetContent())).ToList();

            await promptRepository.TrackPrompt(
                llmPromptDto: llmPromptDto,
                modelEntity: modelResult.Unwrap(),
                accessTokenIdentifier: Guid.Empty,
                providerPromptIdentifier: response.ProviderPromptIdentifier,
                detailedModelIdentifier: response.DetailedModelIdentifierName,
                promptCompletionTime: DateTime.UtcNow - startTime,
                streamed: false,
                llmContent: content,
                llmUsage: response.Usage);
        }

        return clientResult;
    }

    public async IAsyncEnumerable<LlmStreamEvent> PromptStream(
        LlmPromptDto llmPromptDto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelResult = await llmModelRepository
            .GetModel(new ModelEntityId(llmPromptDto.ModelIdentifier));
        if (modelResult.IsError)
        {
            logger.LogCritical(modelResult.Error!, "Failed to get model for prompt");
            yield return new LlmStreamError($"Invalid model identifier");
            yield break;
        }

        LlmUsage? llmUsage = default;
        string? providerPromptIdentifier = default;
        string? detailedModelIdentifier = default;

        var startTime = DateTime.UtcNow;
        var sbDict = new Dictionary<int, (LlmContentType ContentType, StringBuilder StringBuilder)>();
        await foreach (var streamEvent in largeLanguageModelService.PromptStream(llmPromptDto, modelResult.Unwrap(), cancellationToken))
        {
            if (streamEvent is LlmStreamMessageStart)
            {
                var llmStreamMessageStart = streamEvent as LlmStreamMessageStart;
                detailedModelIdentifier = llmStreamMessageStart?.Message.DetailedModelIdentifierName;
            }

            if (streamEvent is LlmStreamContentDelta)
            {
                var llmStreamContentDelta = streamEvent as LlmStreamContentDelta;
                var sbTuple = GetStringBuilderTuple(llmStreamContentDelta!.Index, llmStreamContentDelta.Delta.Type, sbDict);
                sbTuple.StringBuilder.Append(llmStreamContentDelta.Delta.GetContent());
            }

            if (streamEvent is LlmStreamTotalUsage)
            {
                var llmStreamTotalUsage = streamEvent as LlmStreamTotalUsage;
                var outputTokens = llmStreamTotalUsage!.OutputTokens;

                llmUsage = new LlmUsage(
                    InputTokens: llmStreamTotalUsage!.InputTokens,
                    OutputTokens: outputTokens);

                providerPromptIdentifier = llmStreamTotalUsage.ProviderPromptIdentifier;
            }

            yield return streamEvent;
        }

        var contentList = sbDict.Select(x => (ContentType: x.Value.ContentType, Content: x.Value.StringBuilder.ToString())).ToList();
        await promptRepository.TrackPrompt(
            llmPromptDto: llmPromptDto,
            modelEntity: modelResult.Unwrap(),
            accessTokenIdentifier: Guid.Empty,
            providerPromptIdentifier: providerPromptIdentifier,
            detailedModelIdentifier: detailedModelIdentifier,
            promptCompletionTime: DateTime.UtcNow - startTime,
            streamed: true,
            llmContent: contentList,
            llmUsage: llmUsage!);
    }

    private static (LlmContentType ContentType, StringBuilder StringBuilder) GetStringBuilderTuple(int index, LlmContentType contentType, Dictionary<int, (LlmContentType ContentType, StringBuilder StringBuilder)> sbDict)
    {
        if (sbDict.TryGetValue(index, out var sb))
        {
            return sb;
        }

        var item = (ContentType: contentType, StringBuilder: new StringBuilder());
        sbDict.Add(index, item);
        return item;
    }
}
