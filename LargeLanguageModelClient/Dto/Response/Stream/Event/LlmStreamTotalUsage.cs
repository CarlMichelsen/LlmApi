using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamTotalUsage : LlmStreamEvent
{
    [JsonPropertyName("type")]
    public override LlmStreamEventType Type => LlmStreamEventType.TotalUsage;

    [JsonPropertyName("providerPromptIdentifier")]
    public required string ProviderPromptIdentifier { get; init; }

    [JsonPropertyName("inputTokens")]
    public required long InputTokens { get; init; }

    [JsonPropertyName("outputTokens")]
    public required long OutputTokens { get; init; }

    [JsonPropertyName("stopReason")]
    public required string StopReason { get; init; }
}
