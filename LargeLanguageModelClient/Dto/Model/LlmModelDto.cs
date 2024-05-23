namespace LargeLanguageModelClient.Dto.Model;

public record LlmModelDto(
    Guid Id,
    bool Available,
    string ProviderName,
    LlmPriceDto Price,
    string ModelIdentifierName,
    long MaxTokenCount,
    long ContextTokenCount,
    bool ImageSupport,
    bool VideoSupport,
    bool JsonResponseOptimized,
    string ModelDisplayName,
    string ModelDescription,
    DateTime LastUpdatedUtc);