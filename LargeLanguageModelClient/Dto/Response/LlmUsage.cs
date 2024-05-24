using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Response;

public record LlmUsage(
    [property: JsonPropertyName("inputTokens")] long InputTokens,
    [property: JsonPropertyName("outputTokens")] long OutputTokens);
