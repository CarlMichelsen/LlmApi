namespace Domain.Exception;

public class TypedIdException : System.Exception
{
    public TypedIdException(string message)
        : base(message)
    {
    }

    public TypedIdException(string? message, System.Exception? innerException)
        : base(message, innerException)
    {
    }
}