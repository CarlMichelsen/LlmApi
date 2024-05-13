namespace Domain.Dto.Anthropic.Response.Stream;

public class AnthropicStreamMessageStop : AnthropicStreamEvent
{
    public override string Type => "message_stop";
}
