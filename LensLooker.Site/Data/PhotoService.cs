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

    public async Task<PhotosResult> GetPhotos(int? lensId, int pageNumber, int pageSize)
    {
        return await _memoryCache.GetOrCreateAsync(
            CacheKeys.BuildPhotosCacheKey(lensId, pageNumber, pageSize),
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.PhotoCacheExpirationMinutes);
                return await GetPhotosFromDatabase(lensId, pageNumber, pageSize);
            });
    }

    private IEnumerable<IGrouping<string, Lens>> GetLensesFromDatabase()
    {
        var lenses = _dbContext.Lenses
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

    private async Task<PhotosResult> GetPhotosFromDatabase(int? lensId, int pageNumber, int pageSize)
    {
        var lens = await _dbContext.Lenses.FindAsync(lensId);
        var photos = await _dbContext
            .Photos
            .Where(LensPredicate(lens))
            .OrderBy(p => p.Server)
            .Include(e => e.Camera)
            .Include(e => e.Lens)
            .AsNoTracking()
            .ToListAsync();

        var totalCount = photos.Count;
        var pageCount = (int)Math.Max(1, Math.Ceiling(totalCount / (double)pageSize));
        pageNumber = pageNumber <= pageCount && pageNumber > 0 ? pageNumber : 1;

        var skip = (pageNumber - 1) * pageSize;
        return new PhotosResult(totalCount, photos
            .Select(p => p.ToViewModel(ModelExtensions.PhotoSize.Small320))
            .Skip(skip)
            .Take(pageSize));
    }

    private static Expression<Func<Photo, bool>> LensPredicate(Lens? lens)
    {
        return p => p.IsExifFetched && p.LensId.HasValue && p.CameraId.HasValue &&
                    (lens == null || p.LensId == lens.Id);
    }
}