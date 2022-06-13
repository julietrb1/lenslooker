using LensLooker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LensLooker.Data;

public class LensLookerContext : DbContext
{
    public LensLookerContext(DbContextOptions<LensLookerContext> options) : base(options)
    {
    }

    public DbSet<Photo> Photos { get; set; }
    public DbSet<Lens> Lenses { get; set; }
    public DbSet<Camera> Cameras { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<LensFamily> LensFamilies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var canonBrand = new Brand { Id = 1, Name = "Canon" };

        // Brands
        modelBuilder.Entity<Brand>().HasData(canonBrand, new Brand { Id = 2, Name = "Nikon" },
            new Brand { Id = 3, Name = "Sony" });

        // Canon Lens families
        modelBuilder.Entity<LensFamily>().HasData(new
            { Id = 1, Name = "EF", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new
            { Id = 2, Name = "EF L", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new
            { Id = 3, Name = "EF-S", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new
            { Id = 4, Name = "RF", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new
            { Id = 5, Name = "RF L", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.FullFrame });
    }
}