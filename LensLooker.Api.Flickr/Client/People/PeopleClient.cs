using LensLooker.Api.Flickr.Client.Common;
using LensLooker.Api.Flickr.Client.People.Models;
using LensLooker.Api.Flickr.Config;
using LensLooker.Api.Flickr.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LensLooker.Api.Flickr.Client.People;

public class PeopleClient : FlickrClient, IPeopleClient
{
    public PeopleClient(IOptions<FlickrOptions> options, HttpClient httpClient, ILogger<PeopleClient> logger) : base(
        options,
        httpClient,
        logger)
    {
    }

    public async Task<GenericPhotosResponse> GetPhotos(GetPhotosRequest request)
    {
        try
        {
            return await GetWithRateLimiting<GenericPhotosResponse>(Methods.MethodGetPhotos,
                $"user_id={request.UserId}&page={request.Page}&per_page={request.PerPage}");
        }
        catch (FlickrException e)
        {
            throw e.Code switch
            {
                1 => new RequiredArgmentsMissingException(e.Stat, e.Code, e.FlickrMessage),
                2 => new UserNotFoundException(e.Stat, e.Code, e.FlickrMessage),
                5 => new UserDeletedException(e.Stat, e.Code, e.FlickrMessage),
                _ => e
            };
        }
    }

    public async Task<GenericPhotosResponse> GetPublicPhotos(GetPublicPhotosRequest request)
    {
        try
        {
            return await GetWithRateLimiting<GenericPhotosResponse>(Methods.MethodGetPublicPhotos,
                $"user_id={request.UserId}&page={request.Page}&per_page={request.PerPage}");
        }
        catch (FlickrException e)
        {
            throw e.Code switch
            {
                1 => new UserNotFoundException(e.Stat, e.Code, e.FlickrMessage),
                _ => e
            };
        }
    }

    private static class Methods
    {
        public const string MethodGetPhotos = "flickr.people.getPhotos";
        public const string MethodGetPublicPhotos = "flickr.people.getPublicPhotos";
    }
}