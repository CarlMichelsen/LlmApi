using System.Text.Json;
using LargeLanguageModelClient;

namespace Implementation.Json;

public static class JsonSerializerOptionsHelper
{
    public static readonly JsonSerializerOptions LlmContentOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new LlmContentConverter(),
            new LlmStreamEventConverter(),
        },
    };
}
