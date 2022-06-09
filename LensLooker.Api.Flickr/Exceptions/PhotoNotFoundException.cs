namespace LensLooker.Api.Flickr.Exceptions;

public class PhotoNotFoundException : FlickrException
{
    public PhotoNotFoundException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}