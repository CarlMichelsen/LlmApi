namespace Domain.Configuration;

public class AnthropicOptions
{
    public const string SectionName = "Anthropic";

    public required string ApiBaseUrl { get; init; }

    public required Dictionary<string, string> NameKeyPairs { get; init; }
}
