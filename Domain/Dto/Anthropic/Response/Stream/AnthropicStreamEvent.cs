namespace Domain.Dto.Anthropic.Response.Stream;

public abstract class AnthropicStreamEvent
{
    public abstract string Type { get; }
}
