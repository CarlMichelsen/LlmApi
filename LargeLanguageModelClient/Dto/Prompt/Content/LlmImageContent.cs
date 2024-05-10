namespace LargeLanguageModelClient.Dto.Prompt.Content;

public class LlmImageContent : LlmContent
{
    public override LlmContentType Type => LlmContentType.Image;

    public string Format => "base64";

    /// <summary>
    /// Gets media type of the image. Right now only "image/jpeg" is tested.
    /// </summary>
    public required string MediaType { get; init; }

    /// <summary>
    /// Gets image data.
    /// </summary>
    public required string Data { get; init; }
}
