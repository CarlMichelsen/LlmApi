using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Response.Stream;

public class LlmStreamMessageStop : LlmStreamEvent
{
    [JsonPropertyName("type")]
    public override LlmStreamEventType Type => LlmStreamEventType.MessageStop;

    [JsonPropertyName("stopReason")]
    public required string StopReason { get; init; }
}
