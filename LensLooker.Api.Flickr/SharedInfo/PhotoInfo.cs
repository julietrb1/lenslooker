using System.Text.RegularExpressions;

namespace LensLooker.Api.Flickr.SharedInfo;

public static class PhotoInfo
{
    private const string LensPostSpec = @"\s?\d{1,3}(-\d{1,3})?\s?mm";
    public static readonly Regex CanonEfLensRegex = new($"EF{LensPostSpec}");
    public static readonly Regex CanonRfLensRegex = new($"RF{LensPostSpec}");
    public static readonly Regex SonyDtLensRegex = new($"DT{LensPostSpec}");
    public static readonly Regex SonyFeLensRegex = new($"FE{LensPostSpec}");
}