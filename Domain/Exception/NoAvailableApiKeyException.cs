using Domain.Entity;

namespace Domain.Exception;

public class NoAvailableApiKeyException : System.Exception
{
    public NoAvailableApiKeyException(string message, LlmProvider llmProvider)
        : base(message)
    {
        this.LlmProvider = llmProvider;
    }

    public LlmProvider LlmProvider { get; init; }
}
