using System.Runtime.CompilerServices;
using Domain.Abstraction;
using Domain.Entity;
using Domain.Exception;
using Interface.Client;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Implementation.Client;

public class GenericLargeLanguageModelClient(
    ILogger<GenericLargeLanguageModelClient> logger,
    IServiceProvider serviceProvider) : IGenericLlmClient
{
    public async Task<Result<LlmResponse>> Prompt(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        CancellationToken cancellationToken = default)
    {
        var enumResult = this.ToEnum(llmPromptDto.Model.ProviderName);
        if (enumResult.IsError)
        {
            return enumResult.Error!;
        }

        var client = this.GetGenericLlmClient(enumResult.Unwrap());
        return await client.Prompt(llmPromptDto, modelEntity);
    }

    public async IAsyncEnumerable<LlmStreamEvent> PromptStream(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var enumResult = this.ToEnum(llmPromptDto.Model.ProviderName);
        if (enumResult.IsError)
        {
            logger.LogCritical(
                enumResult.Error!,
                "GenericLargeLanguageModelClient Stream failed to parse ProviderName");
            
            yield return new LlmStreamError("Failed to parse ProviderName");
            yield break;
        }

        var client = this.GetGenericLlmClient(enumResult.Unwrap());
        var results = client.PromptStream(llmPromptDto, modelEntity, cancellationToken);
        await foreach (var item in results)
        {
            yield return item;
        }
    }

    public virtual IGenericLlmClient GetGenericLlmClient(LlmProvider llmProvider)
        => llmProvider switch
        {
            LlmProvider.Anthropic => serviceProvider.GetRequiredService<GenericAnthropicClient>(),
            _ => throw new NotImplementedException($"No client implemented for {nameof(llmProvider)}"),
        };

    private Result<LlmProvider> ToEnum(string providerName)
    {
        if (Enum.TryParse<LlmProvider>(providerName, out var provider))
        {
            return provider;
        }

        return new MapException("Failed to map ProviderName to enum");
    }
}
