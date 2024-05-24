using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamError : LlmStreamEvent
{
    public LlmStreamError(string message)
    {
        this.Message = message;
    }

    [JsonPropertyName("type")]
    public override LlmStreamEventType Type => LlmStreamEventType.Error;

    [JsonPropertyName("message")]
    public string Message { get; init; }
}
