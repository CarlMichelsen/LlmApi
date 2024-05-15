using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Dto.OpenAi.Content;

namespace Implementation.Json;

public class OpenAiContentConverter : JsonConverter<OpenAiContent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(OpenAiContent);
    }

    public override OpenAiContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            var root = jsonDoc.RootElement;
            var typeDiscriminator = root.GetProperty("type").GetString();
            var actualType = typeDiscriminator switch
            {
                "text" => typeof(OpenAiTextContent),
                "image_url" => typeof(OpenAiImageContent),
                _ => throw new JsonException($"Unknown AnthropicContent type \"{typeDiscriminator}\"."),
            };

            var rawJson = root.GetRawText();
            return (OpenAiContent)JsonSerializer.Deserialize(rawJson, actualType, options)!;
        }
    }

    public override void Write(Utf8JsonWriter writer, OpenAiContent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
