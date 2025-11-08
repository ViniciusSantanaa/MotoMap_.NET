using Microsoft.EntityFrameworkCore;
using MotoMap.Api.Models;

namespace MotoMap.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Motorcycle> Motorcycles { get; set; } = null!;
        public DbSet<Reader> Readers { get; set; } = null!;
        public DbSet<Yard> Yards { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exemplo: índice único em TagId da moto (ajuste conforme seu modelo)
            if (modelBuilder.Model.FindEntityType(typeof(Motorcycle)) != null)
            {
                modelBuilder.Entity<Motorcycle>()
                    .HasIndex(m => m.TagId)
                    .IsUnique();
            }
        }
    }
}
