using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace LargeLanguageModelClient;

internal class LargeLanguageModelClient(
    HttpClient httpClient) : ILargeLanguageModelClient
{
    private const string PromptPath = "api/v1/prompt";
    private const string StreamPromptPath = "api/v1/prompt/stream";

    public Task<LlmResponse> Prompt(LlmPromptDto llmPromptDto)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<object> PromptStream(LlmPromptDto llmPromptDto)
    {
        throw new NotImplementedException();
    }
}