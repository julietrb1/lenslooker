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

    // TODO: This should really use `canConstructUrl` from the `Photo` model, but that causes LINQ errors. Sad.
    private static Expression<Func<Photo, bool>> UrlInfoPredicate =>
        p => !string.IsNullOrWhiteSpace(p.Server) &&
             !string.IsNullOrWhiteSpace(p.Secret) &&
             !string.IsNullOrWhiteSpace(p.OwnerId);

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

    public async Task<PhotosResult> GetPhotos(string? lensName, int pageNumber, int pageSize)
    {
        return await _memoryCache.GetOrCreateAsync(
            CacheKeys.BuildPhotosCacheKey(lensName, pageNumber, pageSize),
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.PhotoCacheExpirationMinutes);
                return await GetPhotosFromDatabase(lensName, pageNumber, pageSize);
            });
    }

    private IEnumerable<IGrouping<string, Lens>> GetLensesFromDatabase()
    {
        var lenses = _dbContext.Lenses
            .Where(l => l.LensFamily != null)
            .Include(l => l.LensFamily)
            .ThenInclude(f => f!.CameraBrand)
            .Include(l => l.Photos)
            .ToList();

        return lenses
            .GroupBy(l => $"{l.LensFamily!.CameraBrand.Name} {l.LensFamily.Name}")
            .OrderBy(g => g.Key)
            .ThenByDescending(g => g.Count());
    }

    private async Task<PhotosResult> GetPhotosFromDatabase(string? lensName, int pageNumber, int pageSize)
    {
        var lens = await _dbContext.Lenses.FindAsync(lensName);
        var photos = await _dbContext
            .Photos
            .Where(LensPredicate(lens))
            .Where(UrlInfoPredicate)
            .OrderBy(p => p.PhotoId)
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
        return p => p.IsExifFetched && p.Lens != null && p.Camera != null && (lens == null || p.Lens == lens);
    }
}