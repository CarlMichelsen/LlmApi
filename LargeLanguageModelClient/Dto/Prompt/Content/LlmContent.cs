namespace LargeLanguageModelClient.Dto.Prompt.Content;

public abstract class LlmContent
{
    public abstract LlmContentType Type { get; }

    public abstract string GetContent();
}
