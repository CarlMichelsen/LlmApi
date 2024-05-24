using System.Text.Json.Serialization;
using LargeLanguageModelClient.Dto.Prompt;

namespace LargeLanguageModelClient.Dto.Response;

public record LlmResponse(
    [property: JsonPropertyName("providerPromptIdentifier")] string ProviderPromptIdentifier,
    [property: JsonPropertyName("modelId")] Guid ModelId,
    [property: JsonPropertyName("modelIdentifierName")] string ModelIdentifierName,
    [property: JsonPropertyName("message")] LlmPromptMessageDto Message,
    [property: JsonPropertyName("usage")] LlmUsage Usage,
    [property: JsonPropertyName("stopReason")] string? StopReason,
    [property: JsonPropertyName("detailedModelIdentifierName")] string? DetailedModelIdentifierName = default);