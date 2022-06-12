namespace LensLooker.Site.Caching;

public static class CacheKeys
{
    private const string PhotosKey = "photos";
    public const string LensesKey = "lenses";

    public static string BuildPhotosCacheKey(string? lensName, int pageNumber, int pageSize)
    {
        return $"{PhotosKey}::{lensName}::{pageNumber}::{pageSize}";
    }
}