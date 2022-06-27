namespace LensLooker.Site.Data;

public record PhotosResult(
    int TotalPhotos, IEnumerable<PhotoDto> FilteredPhotos);