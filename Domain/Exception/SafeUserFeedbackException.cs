namespace Domain.Exception;

public class SafeUserFeedbackException : System.Exception
{
    public SafeUserFeedbackException(string message, params string[] details)
        : base(message)
    {
        if (details is not null && details.Length > 0)
        {
            this.Details = details.ToList();
        }
    }

    public List<string> Details { get; init; } = new();
}
