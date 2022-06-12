using LensLooker.Api.Flickr.Client.Common;
using LensLooker.Api.Flickr.Client.GroupsPools.Models;

namespace LensLooker.Api.Flickr.Client.GroupsPools;

public interface IGroupsPoolsClient
{
    Task<GenericPhotosResponse> GetPhotos(GroupsPoolsGetPhotosRequest request);
}