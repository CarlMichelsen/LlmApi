namespace Domain.Dto.Anthropic.Content;

public class AnthropicImageContent : AnthropicContent
{
    public override string Type => "image";

    public required AnthropicSource Source { get; init; }
}
