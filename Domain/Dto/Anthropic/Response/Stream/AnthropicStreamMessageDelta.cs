namespace Domain.Dto.Anthropic.Response.Stream;

public class AnthropicStreamMessageDelta : AnthropicStreamEvent
{
    public override string Type => "message_delta";

    public required AnthropicStreamStopReason Delta { get; init; }

    public required AnthropicStreamPartialUsage Usage { get; init; }
}
