namespace LensLooker.Api.Flickr.Exceptions;

public class UnknownFlickrException : FlickrException
{
    public UnknownFlickrException(string stat, int? code = default, string? message = default) : base(stat, code,
        message)
    {
    }
}