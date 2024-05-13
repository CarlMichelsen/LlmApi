namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamError : LlmStreamEvent
{
    public LlmStreamError(string message)
    {
        this.Message = message;
    }

    public override LlmStreamEventType Type => LlmStreamEventType.Error;

    public string Message { get; init; }
}
