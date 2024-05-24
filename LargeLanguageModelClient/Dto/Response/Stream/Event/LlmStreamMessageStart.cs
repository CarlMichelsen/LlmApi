using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamMessageStart : LlmStreamEvent
{
    [JsonPropertyName("type")]
    public override LlmStreamEventType Type => LlmStreamEventType.MessageStart;

    [JsonPropertyName("message")]
    public required LlmResponse Message { get; init; }
}
