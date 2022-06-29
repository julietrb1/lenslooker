namespace LensLooker.Site.Caching;

public static class CacheKeys
{
    private const string PhotosKey = "photos";
    internal const string LensesKey = "lenses";
    private const string FirstPhotoCacheKey = "photos_first";
    private const string LastPhotoCacheKey = "photos_last";

    public static string BuildPhotosCacheKey(int? lensId, int pageSize, int? beforeId = null,
        int? afterId = null)
    {
        return $"{PhotosKey}::{lensId}::{pageSize}::{beforeId}::{afterId}";
    }

    public static string BuildFirstPhotoCacheKey(int? lensId)
    {
        return $"{FirstPhotoCacheKey}::{lensId}";
    }

    public static string BuildLastPhotoCacheKey(int? lensId)
    {
        return $"{LastPhotoCacheKey}::{lensId}";
    }
}