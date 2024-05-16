using Domain.Entity;
using LargeLanguageModelClient;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace Interface.Repository;

public interface IPromptRepository
{
    Task TrackPrompt(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        Guid accessTokenIdentifier,
        string? providerPromptIdentifier,
        string? detailedModelIdentifier,
        TimeSpan promptCompletionTime,
        bool streamed,
        List<(LlmContentType ContentType, string Content)> llmContent,
        LlmUsage llmUsage);
}
