using Domain.Abstraction;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace Interface.Service;

public interface ILargeLanguageModelService
{
    public Task<Result<LlmResponse>> Prompt(LlmPromptDto llmPromptDto);

    public IAsyncEnumerable<Result<object>> PromptStream(LlmPromptDto llmPromptDto);
}
