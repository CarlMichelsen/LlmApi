using System.Text.Json;
using System.Text.Json.Serialization;
using LargeLanguageModelClient.Dto.Prompt.Content;

namespace Implementation.Json;

public class LlmContentConverter : JsonConverter<LlmContent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(LlmContent);
    }

    public override LlmContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            var root = jsonDoc.RootElement;
            var typeDiscriminator = root.GetProperty("type").GetString();
            var actualType = typeDiscriminator switch
            {
                "text" => typeof(LlmTextContent),
                "image" => typeof(LlmImageContent),
                _ => throw new JsonException("Unknown LlmContent type."),
            };

            var rawJson = root.GetRawText();
            return (LlmContent)JsonSerializer.Deserialize(rawJson, actualType, options)!;
        }
    }

    public override void Write(Utf8JsonWriter writer, LlmContent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}