namespace LensLooker.Site.Caching;

public static class CacheKeys
{
    private const string PhotosKey = "photos";
    internal const string LensesKey = "lenses";
    internal const string FirstPhotoCacheKey = "photos_first";
    internal const string LastPhotoCacheKey = "photos_last";

    public static string BuildPhotosCacheKey(int? lensId, int pageSize, int? beforeId = null,
        int? afterId = null)
    {
        return $"{PhotosKey}::{lensId}::{pageSize}::{beforeId}::{afterId}";
    }
}