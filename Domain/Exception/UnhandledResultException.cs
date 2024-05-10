namespace Domain.Exception;

public class UnhandledResultException : System.Exception
{
    public UnhandledResultException(System.Exception? innerException)
        : base("Attempted to unwrap failed result", innerException)
    {
    }
}
