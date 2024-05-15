namespace Domain.Configuration;

public class OpenAiOptions
{
    public const string SectionName = "OpenAi";

    public required string ApiBaseUrl { get; init; }

    public required Dictionary<string, string> NameKeyPairs { get; init; }
}
