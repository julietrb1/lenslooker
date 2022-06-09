namespace LensLooker.Api.Flickr.Exceptions;

public class InvalidApiKeyException : FlickrException
{
    public InvalidApiKeyException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}