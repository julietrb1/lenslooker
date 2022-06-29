namespace LensLooker.Site.Data;

public record PhotosResult(IEnumerable<PhotoDto> FilteredPhotos, bool HasPreviousPage, bool HasNextPage);