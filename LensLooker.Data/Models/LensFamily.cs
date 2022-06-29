using System.ComponentModel.DataAnnotations.Schema;

namespace LensLooker.Data.Models;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class LensFamily
{
    public int Id { get; set; }
    [ForeignKey("CameraBrand")] public int CameraBrandId { get; set; }
    public virtual Brand CameraBrand { get; set; }
    public string Name { get; set; }
    public SensorFormat? SensorFormat { get; set; }
}