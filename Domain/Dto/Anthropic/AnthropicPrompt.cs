namespace Domain.Dto.Anthropic;

public record AnthropicPrompt(
    string Model,
    long MaxTokens,
    string? System,
    List<AnthropicMessage> Messages);
