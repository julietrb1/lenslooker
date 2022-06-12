using LensLooker.Api.Flickr.Client.Common;
using LensLooker.Api.Flickr.Client.People.Models;

namespace LensLooker.Api.Flickr.Client.People;

public interface IPeopleClient
{
    Task<GenericPhotosResponse> GetPhotos(GetPhotosRequest request);
    Task<GenericPhotosResponse> GetPublicPhotos(GetPublicPhotosRequest request);
}