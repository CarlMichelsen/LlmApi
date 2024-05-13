using LargeLanguageModelClient.Dto.Prompt.Content;

namespace LargeLanguageModelClient.Dto.Response.Stream.Event;

public class LlmStreamContentDelta : LlmStreamEvent
{
    public override LlmStreamEventType Type => LlmStreamEventType.ContentDelta;

    public required int Index { get; init; }

    public required LlmContent Delta { get; init; }
}
