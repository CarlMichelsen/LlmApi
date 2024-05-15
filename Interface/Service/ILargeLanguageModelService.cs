using Domain.Abstraction;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;

namespace Interface.Service;

public interface ILargeLanguageModelService
{
    public Task<Result<LlmResponse>> Prompt(
        LlmPromptDto llmPromptDto,
        CancellationToken cancellationToken);

    public IAsyncEnumerable<LlmStreamEvent> PromptStream(
        LlmPromptDto llmPromptDto,
        CancellationToken cancellationToken);
}
