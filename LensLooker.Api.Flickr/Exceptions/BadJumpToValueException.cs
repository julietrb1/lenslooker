namespace LensLooker.Api.Flickr.Exceptions;

public class BadJumpToValueException : FlickrException
{
    public BadJumpToValueException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}