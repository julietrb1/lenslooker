namespace LensLooker.Api.Flickr.Exceptions;

public class FormatNotFoundException : FlickrException
{
    public FormatNotFoundException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}