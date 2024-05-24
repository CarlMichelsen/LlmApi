using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Model;

public record LlmPriceDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("millionInputTokenPrice")] int MillionInputTokenPrice,
    [property: JsonPropertyName("millionOutputTokenPrice")] int MillionOutputTokenPrice);
