using System.Text.Json;
using Implementation.Json;
using Interface.Service;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response.Stream;
using Microsoft.AspNetCore.Http;

namespace Implementation.Service;

public class HttpPromptStreamService(
    IHttpContextAccessor httpContextAccessor,
    ILargeLanguageModelService largeLanguageModelService) : IHttpPromptStreamService
{
    private const string LinebreakCharacterString = "\n";

    public async Task StreamPrompt(LlmPromptDto llmPromptDto, CancellationToken cancellationToken)
    {
        var context = httpContextAccessor.HttpContext;
        var promptStream = largeLanguageModelService.PromptStream(llmPromptDto, cancellationToken);
        await foreach (var item in this.ToStringStream(promptStream))
        {
            await context.Response.WriteAsync(item);
        }
    }

    private async IAsyncEnumerable<string> ToStringStream(IAsyncEnumerable<LlmStreamEvent> llmStreamEvents)
    {
        await foreach (var streamEvent in llmStreamEvents)
        {
            yield return $"event: {(int)streamEvent.Type}\n";

            var eventJson = JsonSerializer.Serialize(streamEvent, JsonSerializerOptionsHelper.LlmContentOptions);
            yield return $"data: {eventJson}\n";

            yield return LinebreakCharacterString;
        }
    }
}
