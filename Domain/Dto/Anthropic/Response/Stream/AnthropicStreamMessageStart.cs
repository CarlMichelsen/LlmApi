namespace Domain.Dto.Anthropic.Response.Stream;

public class AnthropicStreamMessageStart : AnthropicStreamEvent
{
    public override string Type => "message_start";

    public required AnthropicResponse Message { get; init; }
}