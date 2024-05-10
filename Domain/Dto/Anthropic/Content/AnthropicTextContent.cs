namespace Domain.Dto.Anthropic.Content;

public class AnthropicTextContent : AnthropicContent
{
    public override string Type => "text";

    public required string Text { get; init; }
}
