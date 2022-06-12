namespace LensLooker.Api.Flickr.Exceptions;

public class GroupNotFoundException : FlickrException
{
    public GroupNotFoundException(string stat, int? code, string? message) : base(stat, code, message)
    {
    }
}