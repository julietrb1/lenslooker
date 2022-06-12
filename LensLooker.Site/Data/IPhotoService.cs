namespace LensLooker.Site.Data;

internal interface IPhotoService
{
    Dictionary<string, IOrderedEnumerable<LensViewModel>> GetLenses();
    Task<PhotosResult> GetPhotos(string? lensName, int pageNumber, int pageSize);
}