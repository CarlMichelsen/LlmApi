using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Prompt.Content;

public class LlmTextContent : LlmContent
{
    [JsonIgnore]
    public override LlmContentType Type => LlmContentType.Text;

    [JsonPropertyName("text")]
    public required string Text { get; init; }

    public override string GetContent()
    {
        return this.Text;
    }
}
