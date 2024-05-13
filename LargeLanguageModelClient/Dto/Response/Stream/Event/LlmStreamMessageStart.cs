namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamMessageStart : LlmStreamEvent
{
    public override LlmStreamEventType Type => LlmStreamEventType.MessageStart;

    public required LlmResponse Message { get; init; }
}
