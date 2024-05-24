using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Prompt.Content;

public class LlmTextContent : LlmContent
{
    public override LlmContentType Type { get => LlmContentType.Text; }

    [JsonPropertyName("text")]
    public required string Text { get; init; }

    public override string GetContent()
    {
        return this.Text;
    }
}
