namespace LensLooker.Api.Flickr.Exceptions;

public class UserNotFoundException : FlickrException
{
    public UserNotFoundException(string stat, int? code = default, string? message = default) : base(stat, code,
        message)
    {
    }
}