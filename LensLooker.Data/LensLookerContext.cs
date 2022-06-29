using LensLooker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LensLooker.Data;

public class LensLookerContext : DbContext
{
    private static readonly Brand CanonBrand = new() { Id = 1, Name = "Canon" };
    private static readonly Brand SonyBrand = new() { Id = 3, Name = "Sony" };

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
        ConfigurePhoto(modelBuilder);
        ConfigureBrand(modelBuilder);
        ConfigureLensFamily(modelBuilder);
    }

    private static void ConfigureLensFamily(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 1, Name = "EF", CameraBrandId = CanonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 2, Name = "EF L", CameraBrandId = CanonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 3, Name = "EF-S", CameraBrandId = CanonBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 4, Name = "RF", CameraBrandId = CanonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 5, Name = "RF L", CameraBrandId = CanonBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 6, Name = "DT", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 7, Name = "DT G", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 8, Name = "DT ZA", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 9, Name = "E", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 10, Name = "FE", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 11, Name = "FE G", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 12, Name = "FE ZA", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 13, Name = "EF-M", CameraBrandId = CanonBrand.Id, SensorFormat = SensorFormat.Crop });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 14, Name = "SAL", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 15, Name = "SAL G", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
        modelBuilder.Entity<LensFamily>().HasData(new LensFamily
            { Id = 16, Name = "SAL ZA", CameraBrandId = SonyBrand.Id, SensorFormat = SensorFormat.FullFrame });
    }

    private static void ConfigureBrand(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>().HasData(
            CanonBrand,
            new Brand { Id = 2, Name = "Nikon" },
            SonyBrand,
            new Brand { Id = 4, Name = "Leica" },
            new Brand { Id = 5, Name = "Apple" },
            new Brand { Id = 6, Name = "Pentax" }
        );
    }

    private static void ConfigurePhoto(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Photo>()
            .HasIndex(p => new { p.PhotoId, p.LensId, p.IsExifFetched })
            .IncludeProperties(p => p.CameraId);
    }
}