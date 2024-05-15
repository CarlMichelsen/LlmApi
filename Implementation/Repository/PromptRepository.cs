using Interface.Repository;
using LargeLanguageModelClient;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using Microsoft.Extensions.Logging;

namespace Implementation.Repository;

public class PromptRepository(
    ILogger<PromptRepository> logger) : IPromptRepository
{
    public Task TrackPrompt(LlmPromptDto llmPromptDto, List<(LlmContentType ContentType, string Content)> llmContent, LlmUsage llmUsage)
    {
        // TODO: Actually track that a prompt was made.
        foreach (var item in llmContent)
        {
            logger.LogWarning("{ContentType} -> {Content}", Enum.GetName(item.ContentType), item.Content);
        }
        
        return Task.CompletedTask;
    }
}
