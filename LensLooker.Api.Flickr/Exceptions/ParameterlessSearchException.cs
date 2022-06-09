namespace LensLooker.Api.Flickr.Exceptions;

public class ParameterlessSearchException : FlickrException
{
    public ParameterlessSearchException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}