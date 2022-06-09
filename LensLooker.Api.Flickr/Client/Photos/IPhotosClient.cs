using LensLooker.Api.Flickr.Client.People.Models;
using LensLooker.Api.Flickr.Client.Photos.Models;

namespace LensLooker.Api.Flickr.Client.Photos;

public interface IPhotosClient
{
    Task<GetExifResponse> GetExif(GetExifRequest request);
    Task<GenericPhotosResponse> GetRecent(GetRecentRequest request);
    Task<GenericPhotosResponse> GetPopular(GetPopularRequest request);
    Task<GenericPhotosResponse> Search(SearchRequest request);
}