namespace LensLooker.Api.Flickr.Exceptions;

public class WriteOperationFailedException : FlickrException
{
    public WriteOperationFailedException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}