using Domain.Abstraction;
using Domain.Dto.Anthropic;
using Domain.Dto.Anthropic.Response.Stream;

namespace Interface.Client;

public interface IAnthropicClient
{
    Task<Result<AnthropicResponse>> Prompt(
        AnthropicPrompt anthropicPrompt,
        CancellationToken cancellationToken);

    IAsyncEnumerable<Result<AnthropicStreamEvent>> PromptStream(
        AnthropicPrompt anthropicPrompt,
        CancellationToken cancellationToken);
}
