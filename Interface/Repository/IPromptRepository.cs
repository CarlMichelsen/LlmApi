using LargeLanguageModelClient;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace Interface.Repository;

public interface IPromptRepository
{
    Task TrackPrompt(
        LlmPromptDto llmPromptDto,
        List<(LlmContentType ContentType, string Content)> llmContent,
        LlmUsage llmUsage);
}
