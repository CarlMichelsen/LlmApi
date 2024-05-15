namespace Domain.Configuration;

public class AdminOptions
{
    public const string SectionName = "Admin";

    public required string Username { get; init; }
    
    public required string Password { get; init; }
}
