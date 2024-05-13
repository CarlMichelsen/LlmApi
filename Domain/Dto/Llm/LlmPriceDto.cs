namespace Domain;

public record LlmPriceDto(
    Guid Id,
    long MillionInputTokenPrice,
    long MillionOutputTokenPrice);
