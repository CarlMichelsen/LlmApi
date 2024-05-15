using System.Runtime.CompilerServices;
using Domain.Abstraction;
using Domain.Entity;
using Implementation.Map.Llm.Anthropic;
using Interface.Client;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Microsoft.Extensions.Logging;

namespace Implementation.Client;

public class GenericAnthropicClient(
    ILogger<GenericAnthropicClient> logger,
    IAnthropicClient anthropicClient) : IGenericLlmClient
{
    public async Task<Result<LlmResponse>> Prompt(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        CancellationToken cancellationToken)
    {
        var mapper = new AnthropicPromptMapper(modelEntity);

        var anthropicPromptResult = mapper.Map(llmPromptDto);
        if (anthropicPromptResult.IsError)
        {
            return anthropicPromptResult.Error!;
        }

        var anthropicPrompt = anthropicPromptResult.Unwrap();
        var anthropicResult = await anthropicClient.Prompt(anthropicPrompt, cancellationToken);
        if (anthropicResult.IsError)
        {
            return anthropicResult.Error!;
        }

        var llmResponseResult = mapper.Map(anthropicResult.Unwrap());
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
        var mapper = new AnthropicPromptMapper(modelEntity);
        var anthropicPromptResult = mapper.Map(llmPromptDto);
        if (anthropicPromptResult.IsError)
        {
            logger.LogCritical(anthropicPromptResult.Error!, "PromptStream Anthropic Prompt Mapping Result was an error");
            yield return new LlmStreamError("Anthropic PromptStream mapping error. No prompt was made.");
            yield break;
        }

        var anthropicPrompt = anthropicPromptResult.Unwrap();
        var anthropicResult = anthropicClient.PromptStream(anthropicPrompt, cancellationToken);

        var streamMapper = new AnthropicStreamMapper(logger, modelEntity);
        await foreach (var llmStreamEvent in streamMapper.MapToLlmStream(anthropicResult, cancellationToken))
        {
            yield return llmStreamEvent;
        }
    }
}
