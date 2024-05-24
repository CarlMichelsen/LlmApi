using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Prompt.Content;

public class LlmImageContent : LlmContent
{
    public override LlmContentType Type => LlmContentType.Image;

    public string Format => "base64";

    /// <summary>
    /// Gets media type of the image. Right now only "image/jpeg" is tested.
    /// </summary>
    [JsonPropertyName("mediaType")]
    public required string MediaType { get; init; }

    /// <summary>
    /// Gets image data.
    /// </summary>
    [JsonPropertyName("data")]
    public required string Data { get; init; }

    public override string GetContent()
    {
        return this.Data;
    }
}
