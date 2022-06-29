using System.Linq.Expressions;
using LensLooker.Data;
using LensLooker.Data.Models;
using LensLooker.Site.Caching;
using LensLooker.Site.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LensLooker.Site.Data;

internal class PhotoService : IPhotoService
{
    private readonly LensLookerContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly SiteOptions _options;

    public PhotoService(LensLookerContext dbContext, IMemoryCache memoryCache, IOptions<SiteOptions> options)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _options = options.Value;
    }

    public IEnumerable<IGrouping<string, Lens>> GetLenses()
    {
        return _memoryCache.GetOrCreate(
            CacheKeys.LensesKey,
            cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.LensCacheExpirationMinutes);
                return GetLensesFromDatabase();
            });
    }

    public async Task<PhotosResult> GetPhotos(int? lensId, int pageSize, int? beforeId = null, int? afterId = null)
    {
        return await _memoryCache.GetOrCreateAsync(
            CacheKeys.BuildPhotosCacheKey(lensId, pageSize, beforeId, afterId),
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.PhotoCacheExpirationMinutes);
                return await GetPhotosFromDatabase(lensId, pageSize, beforeId, afterId);
            });
    }

    private async Task<Photo?> GetFirstPhoto(IQueryable<Photo?> photosQuery)
    {
        return await _memoryCache.GetOrCreateAsync(
            CacheKeys.FirstPhotoCacheKey,
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.PhotoCacheExpirationMinutes);
                return await photosQuery.FirstOrDefaultAsync();
            });
    }

    private async Task<Photo?> GetLastPhoto(IQueryable<Photo?> photosQuery)
    {
        return await _memoryCache.GetOrCreateAsync(
            CacheKeys.LastPhotoCacheKey,
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.PhotoCacheExpirationMinutes);
                return await photosQuery.LastOrDefaultAsync();
            });
    }

    private IEnumerable<IGrouping<string, Lens>> GetLensesFromDatabase()
    {
        var lenses = _dbContext.Lenses
            .AsNoTracking()
            .Where(l => l.LensFamilyId.HasValue)
            .Include(l => l.LensFamily)
            .ThenInclude(f => f!.CameraBrand)
            .Include(l => l.Photos)
            .ToList();

        return lenses
            .OrderByDescending(l => l.Photos.Count)
            .GroupBy(l => $"{l.LensFamily!.CameraBrand.Name} {l.LensFamily.Name}")
            .OrderBy(g => g.Key);
    }

    private async Task<PhotosResult> GetPhotosFromDatabase(int? lensId, int pageSize, int? beforeId = null,
        int? afterId = null)
    {
        if (beforeId != null && afterId != null)
            throw new ArgumentException(
                $"{nameof(beforeId)} and {nameof(afterId)} provided, but only zero or one arguments allowed.");

        var lens = await _dbContext.Lenses.FindAsync(lensId);
        var photosQuery = _dbContext.Photos
            .OrderBy(p => p.Id)
            .AsNoTracking()
            .Where(LensPredicate(lens));

        var firstPhoto = await GetFirstPhoto(photosQuery);
        var lastPhoto = await GetLastPhoto(photosQuery);

        var photos = await photosQuery
            .Where(p => (beforeId == null && afterId == null) || beforeId != null ? p.Id < beforeId : p.Id > afterId)
            .Take(pageSize)
            .Include(e => e.Camera)
            .Include(e => e.Lens)
            .ToListAsync();

        var hasPreviousPage = firstPhoto != null && !photos.Contains(firstPhoto);
        var hasNextPage = lastPhoto != null && !photos.Contains(lastPhoto);

        var photoDtos = photos
            .Select(p => p.ToPhotoDto(ModelExtensions.PhotoSize.Small320));
        return new PhotosResult(photoDtos, hasPreviousPage, hasNextPage);
    }

    private static Expression<Func<Photo, bool>> LensPredicate(Lens? lens)
    {
        return p => p.IsExifFetched && p.LensId.HasValue && p.CameraId.HasValue &&
                    (lens == null || p.LensId == lens.Id);
    }
}