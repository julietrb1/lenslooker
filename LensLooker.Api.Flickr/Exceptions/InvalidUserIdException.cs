namespace LensLooker.Api.Flickr.Exceptions;

public class InvalidUserIdException : FlickrException
{
    public InvalidUserIdException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}