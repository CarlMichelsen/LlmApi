namespace Domain.Configuration;

public class RedisOptions
{
    public const string SectionName = "Redis";

    public required string InstanceName { get; init; }

    public required string ConnectionString { get; init; }
}
