using Microsoft.EntityFrameworkCore;

namespace LensLooker.Data.Models;

[Index(nameof(Name), IsUnique = true)]
public class Camera
{
    public int Id { get; set; }

    public string Name { get; set; }
    public int? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
}