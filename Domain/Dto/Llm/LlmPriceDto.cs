namespace Domain;

public record LlmPriceDto(
    Guid Id,
    int MillionInputTokenPrice,
    int MillionOutputTokenPrice);
