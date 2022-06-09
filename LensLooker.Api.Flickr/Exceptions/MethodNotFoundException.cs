namespace LensLooker.Api.Flickr.Exceptions;

public class MethodNotFoundException : FlickrException
{
    public MethodNotFoundException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}