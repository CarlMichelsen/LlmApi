using System.Text.Json.Serialization;
using LargeLanguageModelClient.Dto.Response.Stream.Event;

namespace LargeLanguageModelClient.Dto.Response.Stream;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(LlmStreamContentDelta), (int)LlmStreamEventType.ContentDelta)]
[JsonDerivedType(typeof(LlmStreamContentStart), (int)LlmStreamEventType.ContentStart)]
[JsonDerivedType(typeof(LlmStreamContentStop), (int)LlmStreamEventType.ContentStop)]
[JsonDerivedType(typeof(LlmStreamError), (int)LlmStreamEventType.Error)]
[JsonDerivedType(typeof(LlmStreamMessageStart), (int)LlmStreamEventType.MessageStart)]
[JsonDerivedType(typeof(LlmStreamMessageStop), (int)LlmStreamEventType.MessageStop)]
[JsonDerivedType(typeof(LlmStreamTotalUsage), (int)LlmStreamEventType.TotalUsage)]
public abstract class LlmStreamEvent
{
    [JsonPropertyName("typeName")]
    public string TypeName => Enum.GetName(this.Type) ?? "unable to get typename";

    [JsonPropertyName("type")]
    public abstract LlmStreamEventType Type { get; }
}
