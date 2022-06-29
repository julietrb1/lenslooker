namespace LensLooker.Site.Data;

public record PhotoDto(
    int Id,
    string Src,
    string ViewingUrl,
    string? Title = default,
    string? Camera = default,
    string? Lens = default,
    int? FocalLength = default,
    double? Aperture = default,
    string? ShutterSpeed = default
);