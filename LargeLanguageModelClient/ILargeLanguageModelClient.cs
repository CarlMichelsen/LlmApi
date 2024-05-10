using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace LargeLanguageModelClient;

public interface ILargeLanguageModelClient
{
    Task<LlmResponse> Prompt(LlmPromptDto llmPromptDto);

    IAsyncEnumerable<object> PromptStream(LlmPromptDto llmPromptDto);
}