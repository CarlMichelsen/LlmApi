namespace LargeLanguageModelClient.Dto.Response.Stream;

public abstract class LlmStreamEvent
{
    public string TypeName => Enum.GetName(this.Type) ?? "unable to get typename";

    public abstract LlmStreamEventType Type { get; }
}
