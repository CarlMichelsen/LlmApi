namespace LargeLanguageModelClient.Dto;

public class ServiceResponse<T>
{
    public ServiceResponse(T data)
    {
        this.Data = data;
    }

    public ServiceResponse(params string[] errors)
    {
        this.Errors = errors.ToList();
    }

    public bool Ok => this.Errors.Count == 0 && this.Data is not null;

    public T? Data { get; init; }

    public List<string> Errors { get; init; } = new();

    public DateTime ResponseSentUtc => DateTime.UtcNow;
}
