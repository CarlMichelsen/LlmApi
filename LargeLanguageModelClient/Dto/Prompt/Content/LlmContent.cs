using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Prompt.Content;

public abstract class LlmContent
{
    [JsonPropertyName("type")]
    public abstract LlmContentType Type { get; }
    
    public abstract string GetContent();
}
