using System.Linq.Expressions;
using LensLooker.Api.Flickr.SharedInfo;
using LensLooker.Data;
using LensLooker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LensLooker.Site.Data;

internal class PhotoService : IPhotoService
{
    private readonly LensLookerContext _dbContext;

    public PhotoService(LensLookerContext dbContext)
    {
        _dbContext = dbContext;
    }

    // TODO: This should really use `canConstructUrl` from the `Photo` model, but that causes LINQ errors. Sad.
    private static Expression<Func<Photo, bool>> UrlInfoPredicate =>
        p => !string.IsNullOrWhiteSpace(p.Server) &&
             !string.IsNullOrWhiteSpace(p.Secret) &&
             !string.IsNullOrWhiteSpace(p.OwnerId);

    public Dictionary<string, IEnumerable<LensViewModel>> GetLenses()
    {
        var lensEnumerable = _dbContext.Lenses
            .Include(e => e.Photos)
            .ToList();
        return new Dictionary<string, IEnumerable<LensViewModel>>
        {
            ["EF lenses"] = lensEnumerable
                .Where(l => PhotoInfo.EfLensRegex.IsMatch(l.Name))
                .Select(l => l.ToViewModel())
                .Where(vm => vm.PhotoCount > 0)
                .OrderByDescending(vm => vm.PhotoCount),
            ["RF lenses"] = lensEnumerable
                .Where(l => PhotoInfo.RfLensRegex.IsMatch(l.Name))
                .Select(l => l.ToViewModel())
                .Where(vm => vm.PhotoCount > 0)
                .OrderByDescending(vm => vm.PhotoCount)
        };
    }

    public PhotosResult GetPhotos(string? lensName, int pageNumber, int pageSize)
    {
        var lens = _dbContext.Lenses.Find(lensName);
        var photos = _dbContext
            .Photos
            .Where(LensPredicate(lens))
            .Where(UrlInfoPredicate)
            .OrderBy(p => p.PhotoId)
            .Include(e => e.Camera)
            .Include(e => e.Lens)
            .AsNoTracking();

        var totalCount = photos.Count();
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