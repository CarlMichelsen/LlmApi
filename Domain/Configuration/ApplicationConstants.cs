using System.Text.Json;

namespace Domain.Configuration;

public static class ApplicationConstants
{
    public static JsonSerializerOptions DefaultJsonOptions =>
        new(JsonSerializerOptions.Default) { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
}
