namespace Domain.Dto.Anthropic.Response.Stream;

public class AnthropicStreamError : AnthropicStreamEvent
{
    public override string Type => "error";

    public required AnthropicError Error { get; init; }
}
