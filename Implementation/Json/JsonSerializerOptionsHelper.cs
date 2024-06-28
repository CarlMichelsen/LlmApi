using System.Text.Json;

namespace Implementation.Json;

public static class JsonSerializerOptionsHelper
{
    public static readonly JsonSerializerOptions LlmContentOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
