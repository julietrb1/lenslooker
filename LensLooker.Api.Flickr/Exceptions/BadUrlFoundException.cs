namespace LensLooker.Api.Flickr.Exceptions;

public class BadUrlFoundException : FlickrException
{
    public BadUrlFoundException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}