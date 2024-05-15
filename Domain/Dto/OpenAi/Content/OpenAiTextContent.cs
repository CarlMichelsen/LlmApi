namespace Domain.Dto.OpenAi.Content;

public class OpenAiTextContent : OpenAiContent
{
    public override string Type => "text";

    public required string Text { get; init; }
}
