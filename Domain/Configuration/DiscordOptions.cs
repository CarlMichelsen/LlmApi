namespace Domain.Configuration;

public class DiscordOptions
{
    public const string SectionName = "Discord";

    public required string WebhookId { get; init; }

    public required string WebhookToken { get; init; }
}
