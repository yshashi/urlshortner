using Microsoft.EntityFrameworkCore;
using urlshortner.Models;

namespace urlshortner.Persistence.Context;

public class UrlDbContext(DbContextOptions<UrlDbContext> options) : DbContext(options)
{
    public required DbSet<Url> Urls { get; init; }
    public required DbSet<Analytics> Analytics { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Url>().HasKey(u => u.Id);
        modelBuilder.Entity<Url>().Property(u => u.LongUrl).IsRequired();
        modelBuilder.Entity<Url>().Property(u => u.UrlCode).IsRequired();
        modelBuilder.Entity<Url>().Property(u => u.CreatedAt).IsRequired();

        modelBuilder.Entity<Url>()
            .HasMany(a => a.Analytics)
            .WithOne(a => a.Url)
            .HasForeignKey(a => a.UrlId);
        
        modelBuilder.Entity<Url>()
            .HasIndex(u => u.UrlCode)
            .IsUnique();

        modelBuilder.Entity<Analytics>()
            .Property(a => a.ClickedTime)
            .IsRequired();

        modelBuilder.Entity<Analytics>()
            .Property(a => a.UrlCode)
            .IsRequired();

    }
}