namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamTotalUsage : LlmStreamEvent
{
    public override LlmStreamEventType Type => LlmStreamEventType.TotalUsage;

    public required string ProviderPromptIdentifier { get; init; }

    public required long InputTokens { get; init; }

    public required long OutputTokens { get; init; }

    public required string StopReason { get; init; }
}
