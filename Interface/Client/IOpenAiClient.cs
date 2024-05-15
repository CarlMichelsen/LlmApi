using Domain.Abstraction;
using Domain.Dto.OpenAi;
using Domain.Dto.OpenAi.Response.Stream;

namespace Interface.Client;

public interface IOpenAiClient
{
    Task<Result<OpenAiResponse>> Prompt(
        OpenAiPrompt openAiPrompt,
        CancellationToken cancellationToken);

    IAsyncEnumerable<Result<OpenAiStreamEvent>> PromptStream(
        OpenAiPrompt openAiPrompt,
        CancellationToken cancellationToken);
}
