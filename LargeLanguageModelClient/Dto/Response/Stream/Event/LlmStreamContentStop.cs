using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamContentStop : LlmStreamEvent
{
    [JsonPropertyName("type")]
    public override LlmStreamEventType Type => LlmStreamEventType.ContentStop;

    [JsonPropertyName("index")]
    public required int Index { get; init; }
}
