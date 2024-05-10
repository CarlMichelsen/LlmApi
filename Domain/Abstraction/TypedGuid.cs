using Domain.Exception;

namespace Domain.Abstraction;

public abstract class TypedGuid<T>
    where T : TypedGuid<T>
{
    protected TypedGuid(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new TypedIdException("Empty guid not supported for TypedGuid");
        }

        this.Value = value;
    }

    public Guid Value { get; }

    // Equality operators
    public static bool operator ==(TypedGuid<T> left, TypedGuid<T> right) => left.Equals(right);

    public static bool operator !=(TypedGuid<T> left, TypedGuid<T> right) => !(left == right);

    public override string ToString() => this.Value.ToString();

    // Override Equals and GetHashCode for proper value comparison
    public override bool Equals(object? obj)
    {
        if (obj is TypedGuid<T> other)
        {
            return this.Value.Equals(other.Value);
        }

        return false;
    }

    public override int GetHashCode() => this.Value.GetHashCode();
}