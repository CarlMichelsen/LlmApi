using LargeLanguageModelClient.Dto.Prompt;

namespace Interface.Handler;

public interface IStreamPromptHandler
{
    Task StreamPrompt(LlmPromptDto llmPromptDto, CancellationToken cancellationToken);
}
