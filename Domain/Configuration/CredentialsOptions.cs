namespace Domain.Configuration;

public class CredentialsOptions
{
    public const string SectionName = "Credentials";

    public required List<AuthUser> Users { get; init; }
}
