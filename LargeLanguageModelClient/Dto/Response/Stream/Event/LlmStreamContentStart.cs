namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamContentStart : LlmStreamEvent
{
    public override LlmStreamEventType Type => LlmStreamEventType.ContentStart;
    
    public required int Index { get; init; }
}
