using Domain.Entity.Id;

namespace Domain.Entity;

public class PriceEntity
{
    public required PriceEntityId Id { get; init; }

    public required ModelEntityId ModelId { get; init; }

    /// <summary>
    /// Gets price of one million INPUT tokens in cents.
    /// Currency is american dollars.
    /// </summary>
    /// <value>Cents (American dollar).</value>
    public required int MillionInputTokenPrice { get; init; }

    /// <summary>
    /// Gets price of one million OUTPUT tokens in cents.
    /// Currency is american dollars.
    /// </summary>
    /// <value>Cents (American dollar).</value>
    public required int MillionOutputTokenPrice { get; init; }
}
