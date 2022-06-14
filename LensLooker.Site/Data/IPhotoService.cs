using LensLooker.Data.Models;

namespace LensLooker.Site.Data;

internal interface IPhotoService
{
    IEnumerable<IGrouping<string, Lens>> GetLenses();
    Task<PhotosResult> GetPhotos(int? lensId, int pageNumber, int pageSize);
}