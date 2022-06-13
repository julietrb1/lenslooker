using System.ComponentModel.DataAnnotations;

namespace LensLooker.Data.Models;

public class Camera
{
    [Key] public string Name { get; set; }
    public Brand? Brand { get; set; }
}