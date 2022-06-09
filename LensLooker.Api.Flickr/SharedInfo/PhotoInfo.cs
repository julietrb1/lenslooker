using System.Text.RegularExpressions;

namespace LensLooker.Api.Flickr.SharedInfo;

public static class PhotoInfo
{
    public static readonly Regex EfLensRegex = new(@"EF\s?\d{1,3}\s?mm");
    public static readonly Regex RfLensRegex = new(@"RF\s?\d{1,3}\s?mm");
}