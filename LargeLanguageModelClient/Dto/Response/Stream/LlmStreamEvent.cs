using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Response.Stream;

public abstract class LlmStreamEvent
{
    [JsonPropertyName("typeName")]
    public string TypeName => Enum.GetName(this.Type) ?? "unable to get typename";

    [JsonPropertyName("type")]
    public abstract LlmStreamEventType Type { get; }
}
