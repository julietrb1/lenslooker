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
        // Brands
        var canonBrand = new Brand { Id = 1, Name = "Canon" };
        var sonyBrand = new Brand { Id = 3, Name = "Sony" };

        modelBuilder.Entity<Brand>().HasData(
            canonBrand,
            new Brand { Id = 2, Name = "Nikon" },
            sonyBrand,
            new Brand { Id = 4, Name = "Leica" },
            new Brand { Id = 5, Name = "Apple" },
            new Brand { Id = 6, Name = "Pentax" }
        );

        // Canon Lens families
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 1, Name = "EF", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 2, Name = "EF L", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 3, Name = "EF-S", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 4, Name = "RF", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 5, Name = "RF L", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 6, Name = "DT", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 7, Name = "DT G", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 8, Name = "DT ZA", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 9, Name = "E", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 10, Name = "FE", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 11, Name = "FE G", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 12, Name = "FE ZA", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 13, Name = "EF-M", CameraBrandId = canonBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 14, Name = "SAL", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 15, Name = "SAL G", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 16, Name = "SAL ZA", CameraBrandId = sonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
    }
}