using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Model;

public record LlmModelDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("available")] bool Available,
    [property: JsonPropertyName("providerName")] string ProviderName,
    [property: JsonPropertyName("price")] LlmPriceDto Price,
    [property: JsonPropertyName("modelIdentifierName")] string ModelIdentifierName,
    [property: JsonPropertyName("maxTokenCount")] long MaxTokenCount,
    [property: JsonPropertyName("contextTokenCount")] long ContextTokenCount,
    [property: JsonPropertyName("imageSupport")] bool ImageSupport,
    [property: JsonPropertyName("videoSupport")] bool VideoSupport,
    [property: JsonPropertyName("jsonResponseOptimized")] bool JsonResponseOptimized,
    [property: JsonPropertyName("modelDisplayName")] string ModelDisplayName,
    [property: JsonPropertyName("modelDescription")] string ModelDescription,
    [property: JsonPropertyName("lastUpdatedUtc")] DateTime LastUpdatedUtc);