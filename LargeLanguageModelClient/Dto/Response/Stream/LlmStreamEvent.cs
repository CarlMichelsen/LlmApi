namespace LargeLanguageModelClient.Dto.Response.Stream;

public abstract class LlmStreamEvent
{
    public string TypeName => Enum.GetName(this.Type);

    public abstract LlmStreamEventType Type { get; }
}
