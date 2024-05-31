using LensLooker.Api.Flickr.Client.Common;
using LensLooker.Api.Flickr.Client.Models;
using LensLooker.Api.Flickr.Client.People.Models;
using LensLooker.Api.Flickr.Client.Photos.Models;
using LensLooker.Api.Flickr.Config;
using LensLooker.Api.Flickr.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LensLooker.Api.Flickr.Client.Photos;

public class PhotosClient : FlickrClient, IPhotosClient
{
    public PhotosClient(IOptions<FlickrOptions> options, HttpClient httpClient, ILogger<PhotosClient> logger) : base(
        options,
        httpClient,
        logger)
    {
    }

    public async Task<GetExifResponse> GetExif(GetExifRequest request)
    {
        try
        {
            return await GetWithRateLimiting<GetExifResponse>(Methods.MethodGetExif, $"photo_id={request.PhotoId}");
        }
        catch (UnknownFlickrException e)
        {
            throw e.Code switch
            {
                1 => new PhotoNotFoundException(e.Stat, e.Code, e.FlickrMessage),
                2 => new PermissionDeniedException(e.Stat, e.Code, e.FlickrMessage),
                _ => e
            };
        }
    }

    public async Task<GenericPhotosResponse> GetRecent(PaginatedRequest request)
    {
        try
        {
            return await GetWithRateLimiting<GenericPhotosResponse>(Methods.MethodGetRecent,
                $"page={request.Page}&per_page={request.PerPage}");
        }
        catch (FlickrException e)
        {
            throw e.Code switch
            {
                1 => new BadJumpToValueException(e.Stat, e.Code, e.FlickrMessage),
                _ => e
            };
        }
    }

    public async Task<GenericPhotosResponse> GetPopular(PaginatedRequest request)
    {
        try
        {
            return await GetWithRateLimiting<GenericPhotosResponse>(Methods.MethodGetPopular,
                $"page={request.Page}&per_page={request.PerPage}");
        }
        catch (FlickrException e)
        {
            throw e.Code switch
            {
                1 => new InvalidUserIdException(e.Stat, e.Code, e.FlickrMessage),
                2 => new PopularPhotosDisabledException(e.Stat, e.Code, e.FlickrMessage),
                _ => e
            };
        }
    }

    public Task<GenericPhotosResponse> Search(SearchRequest request)
    {
        var query = $"page={request.Page}&per_page={request.PerPage}";
        if (request.Tags != null) query += $"&tags={string.Join(',', request.Tags)}";
        try
        {
            return GetWithRateLimiting<GenericPhotosResponse>(Methods.MethodSearch,
                query);
        }
        catch (FlickrException e)
        {
            throw e.Code switch
            {
                1 => new TooManyTagsException(e.Stat, e.Code, e.FlickrMessage),
                2 => new UserNotFoundException(e.Stat, e.Code, e.FlickrMessage),
                3 => new ParameterlessSearchException(e.Stat, e.Code, e.FlickrMessage),
                _ => e
            };
        }
    }

    private static class Methods
    {
        public const string MethodGetExif = "flickr.photos.getExif";
        public const string MethodGetPopular = "flickr.photos.getPopular";
        public const string MethodGetRecent = "flickr.photos.getRecent";
        public const string MethodSearch = "flickr.photos.search";
    }
}
