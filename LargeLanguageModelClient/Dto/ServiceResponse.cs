using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto;

public class ServiceResponse<T>
{
    public ServiceResponse()
    {
    }

    public ServiceResponse(T data)
    {
        this.Data = data;
    }

    public ServiceResponse(params string[] errors)
    {
        this.Errors = errors.ToList();
    }

    [JsonPropertyName("ok")]
    public bool Ok => this.Errors.Count == 0 && this.Data is not null;

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; init; } = new();

    [JsonPropertyName("responseSentUtc")]
    public DateTime ResponseSentUtc => DateTime.UtcNow;
}
