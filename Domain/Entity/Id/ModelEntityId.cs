using Domain.Abstraction;

namespace Domain.Entity.Id;

public class ModelEntityId : TypedGuid<ModelEntityId>
{
    public ModelEntityId(Guid value)
        : base(value)
    {
    }
}
