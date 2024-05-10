namespace Domain.Dto.Llm;

public record LlmModelDto(
    Guid Id,
    string ProviderName,
    string ModelIdentifierName,
    long MaxTokenCount,
    bool ImageSupport,
    bool VideoSupport,
    bool JsonResponseOptimized,
    string ModelDisplayName,
    string ModelDescription,
    DateTime LastUpdatedUtc);