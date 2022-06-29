using System.ComponentModel.DataAnnotations.Schema;

namespace LensLooker.Data.Models;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Photo
{
    public string PhotoId { get; set; }
    public string OwnerId { get; set; }
    public string Title { get; set; }
    public int Farm { get; set; }
    public string Server { get; set; }
    public string Secret { get; set; }
    public bool IsExifFetched { get; set; }
    public bool IsSkipped { get; set; }
    public double? FNumber { get; set; }
    public string? ExposureTime { get; set; }
    public int? Iso { get; set; }
    public int? FocalLengthInMm { get; set; }

    [ForeignKey("Camera")] public int? CameraId { get; set; }
    public virtual Camera? Camera { get; set; }

    [ForeignKey("Lens")] public int? LensId { get; set; }
    public virtual Lens? Lens { get; set; }
    public DateTime? DateTimeShot { get; set; }
}