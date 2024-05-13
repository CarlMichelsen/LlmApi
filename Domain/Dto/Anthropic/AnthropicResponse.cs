using Domain.Dto.Anthropic.Content;
using Domain.Dto.Anthropic.Response;

namespace Domain.Dto.Anthropic;

public class AnthropicResponse
{
    public required string Id { get; init; }

    public required string Type { get; init; }

    public required string Role { get; init; }

    public required string Model { get; init; }

    public string? StopSequence { get; init; }

    public required AnthropicUsage Usage { get; init; }
    
    public required List<AnthropicContent> Content { get; init; }

    public string? StopReason { get; init; }
}