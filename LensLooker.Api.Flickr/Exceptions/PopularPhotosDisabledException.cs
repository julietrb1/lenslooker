namespace LensLooker.Api.Flickr.Exceptions;

public class PopularPhotosDisabledException : FlickrException
{
    public PopularPhotosDisabledException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}