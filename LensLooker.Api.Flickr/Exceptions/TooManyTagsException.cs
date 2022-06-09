namespace LensLooker.Api.Flickr.Exceptions;

public class TooManyTagsException : FlickrException
{
    public TooManyTagsException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}