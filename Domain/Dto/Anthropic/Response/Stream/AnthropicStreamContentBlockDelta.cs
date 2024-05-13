using Domain.Dto.Anthropic.Content;

namespace Domain.Dto.Anthropic.Response.Stream;

public class AnthropicStreamContentBlockDelta : AnthropicStreamEvent
{
    public override string Type => "content_block_delta";

    public required int Index { get; init; }

    public required AnthropicContent Delta { get; init; }
}
