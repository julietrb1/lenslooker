using System.ComponentModel.DataAnnotations;

namespace LensLooker.Data.Models;

public class Lens
{
    [Key] public string Name { get; set; }
    public List<Photo> Photos { get; set; }
    public Brand? Brand { get; set; }
    public LensFamily? LensFamily { get; set; }
    public Lens? AliasOf { get; set; }
}