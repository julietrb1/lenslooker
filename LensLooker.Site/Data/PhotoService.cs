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
    private readonly ILogger<PhotoService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly SiteOptions _options;

    public PhotoService(LensLookerContext dbContext, IMemoryCache memoryCache, IOptions<SiteOptions> options,
        ILogger<PhotoService> logger)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _logger = logger;
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

    private async Task<int?> GetFirstPhotoId(IQueryable<Photo?> photosQuery, int? lensId)
    {
        return await _memoryCache.GetOrCreateAsync(
            CacheKeys.BuildFirstPhotoCacheKey(lensId),
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.PhotoCacheExpirationMinutes);
                return (await photosQuery.FirstOrDefaultAsync())?.Id;
            });
    }

    private async Task<int?> GetLastPhotoId(IQueryable<Photo?> photosQuery, int? lensId)
    {
        return await _memoryCache.GetOrCreateAsync(
            CacheKeys.BuildLastPhotoCacheKey(lensId),
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.PhotoCacheExpirationMinutes);
                return (await photosQuery.LastOrDefaultAsync())?.Id;
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

        var firstPhotoId = await GetFirstPhotoId(photosQuery, lensId);
        var lastPhotoId = await GetLastPhotoId(photosQuery, lensId);

        var paginatedQuery = GetPaginatedQuery(pageSize, beforeId, afterId, photosQuery);

        var photos = await paginatedQuery
            .Include(e => e.Camera)
            .Include(e => e.Lens)
            .ToListAsync();

        var hasPreviousPage = firstPhotoId != null && !photos.Exists(p => p.Id == firstPhotoId);
        var hasNextPage = lastPhotoId != null && !photos.Exists(p => p.Id == lastPhotoId);

        var photoDtos = photos
            .Select(p => p.ToPhotoDto(ModelExtensions.PhotoSize.Small320));
        return new PhotosResult(photoDtos, hasPreviousPage, hasNextPage);
    }

    private static IQueryable<Photo> GetPaginatedQuery(int pageSize, int? beforeId, int? afterId,
        IQueryable<Photo> photosQuery)
    {
        if (beforeId == null && afterId == null) return photosQuery;
        if (beforeId != null)
            return photosQuery
                .OrderByDescending(p => p.Id)
                .Where(p => p.Id < beforeId)
                .Take(pageSize)
                .Reverse();

        return photosQuery
            .Where(p => p.Id > afterId)
            .Take(pageSize);
    }

    private static Expression<Func<Photo, bool>> LensPredicate(Lens? lens)
    {
        return p => p.IsExifFetched && p.LensId.HasValue && p.CameraId.HasValue &&
                    (lens == null || p.LensId == lens.Id);
    }
}