namespace Domain.Dto.Anthropic.Response;

public class AnthropicError
{
    public required string Type { get; init; }

    public required string Message { get; init; }
}