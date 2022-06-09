namespace LensLooker.Api.Flickr.Exceptions;

public class PermissionDeniedException : FlickrException
{
    public PermissionDeniedException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}