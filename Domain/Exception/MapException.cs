namespace Domain.Exception;

public class MapException : System.Exception
{
    public MapException(string message)
        : base(message)
    {
    }

    public MapException(string? message, System.Exception? innerException)
        : base(message, innerException)
    {
    }
}
