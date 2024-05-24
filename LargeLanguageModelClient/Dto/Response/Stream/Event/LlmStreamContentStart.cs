using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamContentStart : LlmStreamEvent
{
    [JsonPropertyName("type")]
    public override LlmStreamEventType Type => LlmStreamEventType.ContentStart;
    
    [JsonPropertyName("index")]
    public required int Index { get; init; }
}
