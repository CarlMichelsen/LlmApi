using Domain.Abstraction;
using Domain.Entity;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace Interface.Client;

public interface IGenericLlmClient
{
    public Task<Result<LlmResponse>> Prompt(LlmPromptDto llmPromptDto, ModelEntity modelEntity);

    public IAsyncEnumerable<Result<object>> PromptStream(LlmPromptDto llmPromptDto, ModelEntity modelEntity);
}
