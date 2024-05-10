namespace Domain.Dto.Anthropic.Response;

public class AnthropicUsage
{
    public required long InputTokens { get; init; }

    public required long OutputTokens { get; init; }
}
