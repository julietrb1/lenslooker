namespace LensLooker.Site.Data;

public record PhotosResult(
    int totalPhotos, IEnumerable<PhotoViewModel> filteredPhotos);