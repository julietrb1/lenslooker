using LensLooker.Api.Flickr.Client.Common;
using LensLooker.Api.Flickr.Client.GroupsPools.Models;
using LensLooker.Api.Flickr.Config;
using LensLooker.Api.Flickr.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LensLooker.Api.Flickr.Client.GroupsPools;

public class GroupsPoolsClient : FlickrClient, IGroupsPoolsClient
{
    public GroupsPoolsClient(IOptions<FlickrOptions> options, HttpClient httpClient,
        ILogger<GroupsPoolsClient> logger) : base(
        options,
        httpClient,
        logger)
    {
    }

    public async Task<GenericPhotosResponse> GetPhotos(GroupsPoolsGetPhotosRequest request)
    {
        try
        {
            return await GetWithRateLimiting<GenericPhotosResponse>(Methods.MethodGetPhotos,
                $"group_id={request.GroupId}&page={request.Page}&per_page={request.PerPage}");
        }
        catch (FlickrException e)
        {
            throw e.Code switch
            {
                1 => new GroupNotFoundException(e.Stat, e.Code, e.FlickrMessage),
                2 => new PermissionDeniedException(e.Stat, e.Code, e.FlickrMessage),
                3 => new UserNotFoundException(e.Stat, e.Code, e.FlickrMessage),
                _ => e
            };
        }
    }

    private static class Methods
    {
        public const string MethodGetPhotos = "flickr.groups.pools.getPhotos";
    }
}