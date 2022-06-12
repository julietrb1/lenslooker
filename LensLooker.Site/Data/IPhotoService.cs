namespace LensLooker.Site.Data;

internal interface IPhotoService
{
    Dictionary<string, IEnumerable<LensViewModel>> GetLenses();
    Task<PhotosResult> GetPhotos(string? lensName, int pageNumber, int pageSize);
}