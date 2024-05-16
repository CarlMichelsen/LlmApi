namespace Domain.Entity;

public class PromptEntity
{
    public long Id { get; set; }

    public required Guid AccessTokenIdentifier { get; init; }

    public required string? ProviderPromptIdentifier { get; init; }

    public required Guid InternalModelIdentifier { get; init; }

    public required string Model { get; init; }

    public required bool Streamed { get; init; }

    public required TimeSpan PromptCompletionTime { get; init; }

    /// <summary>
    /// Gets price of one million INPUT tokens in cents.
    /// Currency is american dollars.
    /// </summary>
    /// <value>Cents (American dollar).</value>
    public required int CurrentMillionInputTokenPrice { get; init; }

    /// <summary>
    /// Gets price of one million OUTPUT tokens in cents.
    /// Currency is american dollars.
    /// </summary>
    /// <value>Cents (American dollar).</value>
    public required int CurrentMillionOutputTokenPrice { get; init; }

    public required long InputTokens { get; init; }

    public required long OutputTokens { get; init; }

    public required string? SystemMessage { get; init; }

    public required List<PromptMessageEntity> Messages { get; init; }

    public required PromptMessageEntity ResponseMessage { get; init; }

    public required DateTime PromptFinnishedUtc { get; init; }
}
