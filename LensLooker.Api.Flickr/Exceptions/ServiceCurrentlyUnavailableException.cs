namespace LensLooker.Api.Flickr.Exceptions;

public class ServiceCurrentlyUnavailableException : FlickrException
{
    public ServiceCurrentlyUnavailableException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}