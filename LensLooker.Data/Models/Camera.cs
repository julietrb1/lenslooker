using System.ComponentModel.DataAnnotations;

namespace LensLooker.Data.Models;

public class Camera
{
    [Key] public string Name { get; set; }
    public int? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
}