namespace LargeLanguageModelClient.Dto.Response.Stream;

public class LlmStreamMessageStop : LlmStreamEvent
{
    public override LlmStreamEventType Type => LlmStreamEventType.MessageStop;

    public required string StopReason { get; init; }
}
