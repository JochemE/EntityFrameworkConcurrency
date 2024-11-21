using Microsoft.EntityFrameworkCore;

namespace SeedGeneration;

public record DomainEntity
{
    /// <summary>
    /// Unique identity (database generated)
    /// </summary>
    public int? Id { get; init; }
    
    /// <summary>
    /// Functional key for the entity (domain generated)
    /// </summary>
    public required string Number { get; init; }
    
    /// <summary>
    /// The awesome title of this entity
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Reference to the instance that created the entity
    /// </summary>
    public string? InstanceId { get; set; }
}

public record Seed
{
    /// <summary>
    /// Functional key for the seed number
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// Latest number used for data seeding, increment with one when used
    /// </summary>
    public required int Number { get; set; }
}

public class ApplicationDatabaseContext : DbContext
{
    public DbSet<DomainEntity> DomainEntities { get; set; }
    public DbSet<Seed> Seeds { get; set; }

    public ApplicationDatabaseContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "example.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DomainEntity>(e =>
        {
            e.HasKey(x => x.Id);
            
            e.Property(x => x.Number).HasMaxLength(255).IsRequired();
            e.Property(x => x.Title).HasMaxLength(255);
            e.Property(x => x.InstanceId).HasMaxLength(255);
            
            // NOTE: this index causes sql server to throw unique constraint exceptions
            //       without we would get concurrency exceptions on the seed update in the same transaction
            e.HasIndex(x => x.Number).IsUnique();
        });
        
        modelBuilder.Entity<Seed>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Number)
                .IsRequired()
                .IsConcurrencyToken();
        });
    }

    public string DbPath { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => 
        options.UseSqlite($"Data Source={DbPath}");
}