using LensLooker.Api.Flickr.Client.Common;
using LensLooker.Api.Flickr.Client.Models;
using LensLooker.Api.Flickr.Client.People.Models;
using LensLooker.Api.Flickr.Client.Photos.Models;

namespace LensLooker.Api.Flickr.Client.Photos;

public interface IPhotosClient
{
    Task<GetExifResponse> GetExif(GetExifRequest request);
    Task<GenericPhotosResponse> GetRecent(PaginatedRequest request);
    Task<GenericPhotosResponse> GetPopular(PaginatedRequest request);
    Task<GenericPhotosResponse> Search(SearchRequest request);
}
