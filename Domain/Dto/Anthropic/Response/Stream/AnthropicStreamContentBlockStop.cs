namespace Domain.Dto.Anthropic.Response.Stream;

public class AnthropicStreamContentBlockStop : AnthropicStreamEvent
{
    public override string Type => "content_block_stop";

    public required int Index { get; init; }
}
