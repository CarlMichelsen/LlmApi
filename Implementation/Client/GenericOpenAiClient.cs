using System.Runtime.CompilerServices;
using Domain.Abstraction;
using Domain.Entity;
using Implementation.Map.Llm.OpenAi;
using Interface.Client;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Microsoft.Extensions.Logging;

namespace Implementation.Client;

public class GenericOpenAiClient(
    ILogger<GenericOpenAiClient> logger,
    IOpenAiClient openAiClient) : IGenericLlmClient
{
    public async Task<Result<LlmResponse>> Prompt(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        CancellationToken cancellationToken)
    {
        var mapper = new OpenAiPromptMapper(modelEntity);

        var openAiPromptResult = await mapper.Map(llmPromptDto);
        if (openAiPromptResult.IsError)
        {
            return openAiPromptResult.Error!;
        }

        var openAiPrompt = openAiPromptResult.Unwrap();
        var openAiResult = await openAiClient.Prompt(openAiPrompt, cancellationToken);
        if (openAiResult.IsError)
        {
            return openAiResult.Error!;
        }

        var llmResponseResult = mapper.Map(openAiResult.Unwrap());
        if (llmResponseResult.IsError)
        {
            return llmResponseResult.Error!;
        }

        return llmResponseResult.Unwrap();
    }

    public async IAsyncEnumerable<LlmStreamEvent> PromptStream(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var mapper = new OpenAiPromptMapper(modelEntity);
        var openAiPromptResult = await mapper.Map(llmPromptDto);
        if (openAiPromptResult.IsError)
        {
            logger.LogCritical(openAiPromptResult.Error!, "PromptStream OpenAi Prompt Mapping Result was an error");
            yield return new LlmStreamError("OpenAi PromptStream mapping error. No prompt was made.");
            yield break;
        }

        var streamMapper = new OpenAiStreamMapper(logger, modelEntity, llmPromptDto);
        var openAiPrompt = openAiPromptResult.Unwrap();
        var openAiResult = openAiClient.PromptStream(openAiPrompt, cancellationToken);
        await foreach (var llmStreamEvent in streamMapper.MapToLlmStream(openAiResult, cancellationToken))
        {
            yield return llmStreamEvent;
        }
    }
}
