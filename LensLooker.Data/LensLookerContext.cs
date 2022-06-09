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
}