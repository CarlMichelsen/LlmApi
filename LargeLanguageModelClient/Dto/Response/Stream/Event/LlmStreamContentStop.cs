namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamContentStop : LlmStreamEvent
{
    public override LlmStreamEventType Type => LlmStreamEventType.ContentStop;

    public required int Index { get; init; }
}
