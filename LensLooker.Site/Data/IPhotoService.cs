using LensLooker.Data.Models;

namespace LensLooker.Site.Data;

internal interface IPhotoService
{
    IEnumerable<IGrouping<LensFamily, Lens>> GetLenses();
    Task<PhotosResult> GetPhotos(string? lensName, int pageNumber, int pageSize);
}