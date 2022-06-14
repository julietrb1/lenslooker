using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LensLooker.Data.Models;

public class Camera
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Key] public string Name { get; set; }
    public int? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
}