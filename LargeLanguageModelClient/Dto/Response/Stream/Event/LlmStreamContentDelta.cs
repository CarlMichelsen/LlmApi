using System.Text.Json.Serialization;
using LargeLanguageModelClient.Dto.Prompt.Content;

namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamContentDelta : LlmStreamEvent
{
    [JsonPropertyName("type")]
    public override LlmStreamEventType Type => LlmStreamEventType.ContentDelta;

    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required LlmContent Delta { get; init; }
}
