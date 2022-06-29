using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LensLooker.Data.Models;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
[Index(nameof(Name), IsUnique = true)]
public class Camera
{
    public int Id { get; set; }

    public string Name { get; set; }
    [ForeignKey("Brand")] public int? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
}