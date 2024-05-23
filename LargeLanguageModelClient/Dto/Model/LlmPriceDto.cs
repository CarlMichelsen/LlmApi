namespace LargeLanguageModelClient.Dto.Model;

public record LlmPriceDto(
    Guid Id,
    int MillionInputTokenPrice,
    int MillionOutputTokenPrice);
