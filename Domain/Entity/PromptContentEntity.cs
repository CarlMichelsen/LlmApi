namespace Domain.Entity;

public class PromptContentEntity
{
    public long Id { get; set; }

    public required string ContentType { get; init; }

    public required string Content { get; init; }
}
