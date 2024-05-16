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

    public DbSet<PromptEntity> PromptEntity { get; init; }

    public DbSet<ModelEntity> ModelEntity { get; init; }
    
    public DbSet<PriceEntity> PriceEntity { get; init; }

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
            
            entity
                .HasOne(m => m.Price)
                .WithOne()
                .HasForeignKey(nameof(Domain.Entity.ModelEntity))
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PriceEntity>(entity =>
        {
            // Configure the primary key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasConversion(
                    id => id.Value, // How to convert to Guid
                    guid => new PriceEntityId(guid)); // How to convert from Guid

            entity.HasKey(e => e.ModelId);
            entity
                .Property(e => e.ModelId)
                .HasConversion(
                    id => id.Value,
                    guid => new ModelEntityId(guid));
        });
    }
}
