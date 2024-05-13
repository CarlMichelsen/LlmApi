using Domain.Dto.Anthropic.Content;

namespace Domain.Dto.Anthropic.Response.Stream;

public class AnthropicStreamContentBlockStart : AnthropicStreamEvent
{
    public override string Type => "content_block_start";

    public required int Index { get; init; }

    public required AnthropicContent ContentBlock { get; init; }
}
