using Domain.Abstraction;
using Domain.Entity;
using Domain.Exception;
using Interface.Client;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using Microsoft.Extensions.DependencyInjection;

namespace Implementation.Client;

public class GenericLargeLanguageModelClient(
    IServiceProvider serviceProvider) : IGenericLlmClient
{
    public async Task<Result<LlmResponse>> Prompt(LlmPromptDto llmPromptDto, ModelEntity modelEntity)
    {
        var enumResult = this.ToEnum(llmPromptDto.Model.ProviderName);
        if (enumResult.IsError)
        {
            return enumResult.Error!;
        }

        var client = this.GetGenericLlmClient(enumResult.Unwrap());
        return await client.Prompt(llmPromptDto, modelEntity);
    }

    public IAsyncEnumerable<Result<object>> PromptStream(LlmPromptDto llmPromptDto, ModelEntity modelEntity)
    {
        throw new NotImplementedException();
    }

    private Result<LlmProvider> ToEnum(string providerName)
    {
        if (Enum.TryParse<LlmProvider>(providerName, out var provider))
        {
            return provider;
        }

        return new MapException("Failed to map ProviderName to enum");
    }

    private IGenericLlmClient GetGenericLlmClient(LlmProvider llmProvider)
        => llmProvider switch
        {
            LlmProvider.Anthropic => serviceProvider.GetRequiredService<GenericAnthropicClient>(),
            _ => throw new NotImplementedException($"No client implemented for {nameof(llmProvider)}"),
        };
}
