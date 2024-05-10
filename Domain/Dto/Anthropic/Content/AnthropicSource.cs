namespace Domain.Dto.Anthropic.Content;

public class AnthropicSource
{
    public required string Type { get; init; }
    
    public required string MediaType { get; init; }

    public required string Data { get; init; }
}
