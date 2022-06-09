namespace LensLooker.Api.Flickr.Exceptions;

public class UserDeletedException : FlickrException
{
    public UserDeletedException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}