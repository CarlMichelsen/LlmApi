using Interface.Handler;
using Interface.Service;
using LargeLanguageModelClient.Dto.Prompt;

namespace Implementation.Handler;

public class StreamPromptHandler(
    IHttpPromptStreamService httpPromptStreamService) : IStreamPromptHandler
{
    public async Task StreamPrompt(
        LlmPromptDto llmPromptDto,
        CancellationToken cancellationToken)
    {
        await httpPromptStreamService.StreamPrompt(llmPromptDto, cancellationToken);
    }
}
