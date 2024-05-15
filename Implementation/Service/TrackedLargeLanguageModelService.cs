using System.Runtime.CompilerServices;
using System.Text;
using Domain.Abstraction;
using Interface.Repository;
using Interface.Service;
using LargeLanguageModelClient;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;

namespace Implementation.Service;

public class TrackedLargeLanguageModelService(
    LargeLanguageModelService largeLanguageModelService,
    IPromptRepository promptRepository) : ILargeLanguageModelService
{
    public async Task<Result<LlmResponse>> Prompt(
        LlmPromptDto llmPromptDto,
        CancellationToken cancellationToken)
    {
        var clientResult = await largeLanguageModelService.Prompt(llmPromptDto, cancellationToken);
        if (clientResult.IsSuccess)
        {
            var response = clientResult.Unwrap();
            var content = response.Message.Content.Select(x => (ContentType: x.Type, Content: x.GetContent())).ToList();

            await promptRepository.TrackPrompt(
                llmPromptDto,
                content,
                response.Usage);
        }

        return clientResult;
    }

    public async IAsyncEnumerable<LlmStreamEvent> PromptStream(
        LlmPromptDto llmPromptDto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        LlmUsage? llmUsage = default;
        var sbDict = new Dictionary<int, (LlmContentType ContentType, StringBuilder StringBuilder)>();
        await foreach (var streamEvent in largeLanguageModelService.PromptStream(llmPromptDto, cancellationToken))
        {
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
                if (outputTokens is null)
                {
                    outputTokens = llmStreamTotalUsage.EstimatedOutputTokens;
                }

                llmUsage = new LlmUsage(
                    InputTokens: llmStreamTotalUsage!.InputTokens,
                    OutputTokens: (long)outputTokens);
            }

            yield return streamEvent;
        }

        var contentList = sbDict.Select(x => (ContentType: x.Value.ContentType, Content: x.Value.StringBuilder.ToString())).ToList();
        await promptRepository.TrackPrompt(
            llmPromptDto,
            contentList,
            llmUsage!);
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
