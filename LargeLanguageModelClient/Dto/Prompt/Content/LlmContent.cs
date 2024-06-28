using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Prompt.Content;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(LlmImageContent), (int)LlmContentType.Image)]
[JsonDerivedType(typeof(LlmTextContent), (int)LlmContentType.Text)]
public abstract class LlmContent
{
    [JsonPropertyName("type")]
    public abstract LlmContentType Type { get; }
    
    public abstract string GetContent();
}
