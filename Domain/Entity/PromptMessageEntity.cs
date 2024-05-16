namespace Domain.Entity;

public class PromptMessageEntity
{
    public long Id { get; init; }

    public required bool IsUserMessage { get; init; }

    public required List<PromptContentEntity> Content { get; init; }
}
