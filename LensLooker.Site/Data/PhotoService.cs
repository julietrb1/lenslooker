using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LensLooker.Api.Flickr.SharedInfo;
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

    private readonly Dictionary<string, Regex> _lensRegexPairs = new()
    {
        ["Canon EF"] = PhotoInfo.CanonEfLensRegex,
        ["Canon RF"] = PhotoInfo.CanonRfLensRegex,
        ["Sony DT"] = PhotoInfo.SonyDtLensRegex,
        ["Sony FE"] = PhotoInfo.SonyFeLensRegex
    };

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

    public Dictionary<string, IOrderedEnumerable<LensViewModel>> GetLenses()
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

    private Dictionary<string, IOrderedEnumerable<LensViewModel>> GetLensesFromDatabase()
    {
        var lensList = _dbContext.Lenses
            .Include(e => e.Photos)
            .ToList();
        return _lensRegexPairs.ToDictionary(p => p.Key, p => lensList
            .Where(l => p.Value.IsMatch(l.Name))
            .Select(l => l.ToViewModel())
            .Where(vm => vm.PhotoCount > 0)
            .OrderByDescending(vm => vm.PhotoCount));
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
        return p => p.IsExifFetched && (lens == null || p.Lens == lens);
    }
}