using LargeLanguageModelClient.Dto.Prompt;

namespace Interface.Service;

public interface IHttpPromptStreamService
{
    Task StreamPrompt(LlmPromptDto llmPromptDto, CancellationToken cancellationToken);
}
