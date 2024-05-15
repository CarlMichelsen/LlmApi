namespace Domain.Dto.OpenAi.Content;

public class OpenAiImageContent : OpenAiContent
{
    public override string Type => "image_url";

    public required ImageUrl ImageUrl { get; init; }
}
