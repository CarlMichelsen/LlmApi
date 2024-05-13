using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Dto.Anthropic.Content;

namespace Implementation.Json;

public class AnthropicContentConverter : JsonConverter<AnthropicContent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(AnthropicContent);
    }

    public override AnthropicContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            var root = jsonDoc.RootElement;
            var typeDiscriminator = root.GetProperty("type").GetString();
            var actualType = typeDiscriminator switch
            {
                "text_delta" => typeof(AnthropicTextContent),
                "text" => typeof(AnthropicTextContent),
                "image" => typeof(AnthropicImageContent),
                _ => throw new JsonException($"Unknown AnthropicContent type \"{typeDiscriminator}\"."),
            };

            var rawJson = root.GetRawText();
            return (AnthropicContent)JsonSerializer.Deserialize(rawJson, actualType, options)!;
        }
    }

    public override void Write(Utf8JsonWriter writer, AnthropicContent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}