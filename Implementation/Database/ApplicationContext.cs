using Domain.Entity;
using Domain.Entity.Id;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Database;

/// <summary>
/// EntityFramework application context.
/// </summary>
public sealed class ApplicationContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
    /// </summary>
    /// <param name="options">Options for datacontext.</param>
    public ApplicationContext(
        DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<ModelEntity> Models { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("LlmApi");
        
        modelBuilder.Entity<ModelEntity>(entity =>
        {
            // Configure the primary key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasConversion(
                    id => id.Value, // How to convert to Guid
                    guid => new ModelEntityId(guid)); // How to convert from Guid
        });
    }
}
