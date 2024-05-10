using Domain.Dto.Anthropic.Content;

namespace Domain.Dto.Anthropic;

public record AnthropicMessage(
    string Role,
    List<AnthropicContent> Content);
