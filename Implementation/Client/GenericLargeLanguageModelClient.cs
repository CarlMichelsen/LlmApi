using System.Runtime.CompilerServices;
using Domain.Abstraction;
using Domain.Entity;
using Interface.Client;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using Microsoft.Extensions.DependencyInjection;

namespace Implementation.Client;

public class GenericLargeLanguageModelClient(
    IServiceProvider serviceProvider) : IGenericLlmClient
{
    public async Task<Result<LlmResponse>> Prompt(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        CancellationToken cancellationToken)
    {
        var client = this.GetGenericLlmClient(modelEntity.Provider);
        return await client.Prompt(llmPromptDto, modelEntity, cancellationToken);
    }

    public async IAsyncEnumerable<LlmStreamEvent> PromptStream(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = this.GetGenericLlmClient(modelEntity.Provider);
        await foreach (var item in client.PromptStream(llmPromptDto, modelEntity, cancellationToken))
        {
            yield return item;
        }
    }

    public virtual IGenericLlmClient GetGenericLlmClient(LlmProvider llmProvider)
        => llmProvider switch
        {
            LlmProvider.Anthropic => serviceProvider.GetRequiredService<GenericAnthropicClient>(),
            LlmProvider.OpenAi => serviceProvider.GetRequiredService<GenericOpenAiClient>(),
            _ => throw new NotImplementedException($"No client implemented for {Enum.GetName(llmProvider)}"),
        };
}
