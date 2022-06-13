namespace LensLooker.Data.Models;

public class LensFamily
{
    public int Id { get; set; }
    public Brand CameraBrand { get; set; }
    public string Name { get; set; }
    public SensorFormat? SensorFormat { get; set; }
}