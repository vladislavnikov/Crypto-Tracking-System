using CryptoTrackingSystem.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoTrackingSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PriceRecord> PriceRecords => Set<PriceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PriceRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Symbol).HasMaxLength(20).IsRequired();
            e.Property(x => x.Price).HasColumnType("decimal(18,8)");
            e.HasIndex(x => new { x.Symbol, x.Timestamp });
        });
    }
}
