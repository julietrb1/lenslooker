namespace LensLooker.Site.Caching;

public static class CacheKeys
{
    private const string PhotosKey = "photos";
    public const string LensesKey = "lenses";

    public static string BuildPhotosCacheKey(int? lensId, int pageNumber, int pageSize)
    {
        return $"{PhotosKey}::{lensId}::{pageNumber}::{pageSize}";
    }
}