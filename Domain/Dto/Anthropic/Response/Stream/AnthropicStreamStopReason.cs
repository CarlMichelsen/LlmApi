namespace Domain.Dto.Anthropic.Response.Stream;

public record AnthropicStreamStopReason(string StopReason, string? StopSequence);
