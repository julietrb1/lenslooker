namespace LensLooker.Api.Flickr.Exceptions;

public class RequiredArgmentsMissingException : FlickrException
{
    public RequiredArgmentsMissingException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}