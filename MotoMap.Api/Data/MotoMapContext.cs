using Microsoft.EntityFrameworkCore;
using MotoMap.Api.Models;

namespace MotoMap.Api.Data;

public class MotoMapContext : DbContext
{
    public MotoMapContext(DbContextOptions<MotoMapContext> options) : base(options) { }

    public DbSet<Yard> Yards { get; set; } = null!;
    public DbSet<Reader> Readers { get; set; } = null!;
    public DbSet<Motorcycle> Motorcycles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Motorcycle>()
            .HasIndex(m => m.TagId)
            .IsUnique();
    }
}
