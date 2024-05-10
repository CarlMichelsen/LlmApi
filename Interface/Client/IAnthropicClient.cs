using Domain.Dto.Anthropic;

namespace Interface.Client;

public interface IAnthropicClient
{
    Task<AnthropicResponse> Prompt(AnthropicPrompt anthropicPrompt);
}
