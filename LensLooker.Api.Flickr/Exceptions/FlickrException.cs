namespace LensLooker.Api.Flickr.Exceptions;

public abstract class FlickrException : Exception
{
    protected FlickrException(string stat, int? code, string? message) : base(
        $"Flickr returned {stat} with code {code}. Message: {message}")
    {
        Stat = stat;
        Code = code;
        FlickrMessage = message;
    }

    public string? FlickrMessage { get; }

    public int? Code { get; }

    public string Stat { get; }

    public static FlickrException GenerateException(string stat, int? code, string? message)
    {
        return code switch
        {
            100 => new InvalidApiKeyException(stat, code, message),
            105 => new ServiceCurrentlyUnavailableException(stat, code, message),
            106 => new WriteOperationFailedException(stat, code, message),
            111 => new FormatNotFoundException(stat, code, message),
            112 => new MethodNotFoundException(stat, code, message),
            116 => new BadUrlFoundException(stat, code, message),
            _ => new UnknownFlickrException(stat, code, message)
        };
    }
}