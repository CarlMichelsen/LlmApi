using Domain.Abstraction;

namespace Domain.Entity.Id;

public class PriceEntityId : TypedGuid<PriceEntityId>
{
    public PriceEntityId(Guid value)
        : base(value)
    {
    }
}
