using System.Text.Json;
using System.Text.Json.Serialization;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;

namespace Implementation.Json;

public class LlmStreamEventConverter : JsonConverter<LlmStreamEvent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(LlmStreamEvent);
    }

    public override LlmStreamEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            var root = jsonDoc.RootElement;
            var contentType = (LlmStreamEventType)root.GetProperty("type").GetInt32();
            var actualType = contentType switch
            {
                LlmStreamEventType.MessageStart => typeof(LlmStreamMessageStart),
                LlmStreamEventType.ContentStart => typeof(LlmStreamContentStart),
                LlmStreamEventType.ContentDelta => typeof(LlmStreamContentDelta),
                LlmStreamEventType.ContentStop => typeof(LlmStreamContentStop),
                LlmStreamEventType.MessageStop => typeof(LlmStreamMessageStop),
                LlmStreamEventType.TotalUsage => typeof(LlmStreamTotalUsage),
                LlmStreamEventType.Error => typeof(LlmStreamError),
                _ => throw new JsonException("Unknown LlmStreamEventType when reading json using LlmStreamEventConverter."),
            };

            var rawJson = root.GetRawText();
            return (LlmStreamEvent)JsonSerializer.Deserialize(rawJson, actualType, options)!;
        }
    }

    public override void Write(Utf8JsonWriter writer, LlmStreamEvent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
