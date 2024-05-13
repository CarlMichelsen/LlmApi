using Domain.Entity.Id;

namespace Domain.Entity;

public class ModelEntity
{
    public required ModelEntityId Id { get; init; }

    public required LlmProvider Provider { get; init; }

    public required PriceEntity Price { get; init; }

    public required string ModelIdentifierName { get; init; }

    public required long MaxTokenCount { get; init; }

    public required bool ImageSupport { get; init; }

    public required bool VideoSupport { get; init; }

    public required bool JsonResponseOptimized { get; init; }

    public required string ModelDisplayName { get; init; }

    public required string ModelDescription { get; init; }

    public required DateTime LastUpdatedUtc { get; init; }
}
