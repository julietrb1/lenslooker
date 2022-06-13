using System.Text.RegularExpressions;

namespace LensLooker.Api.Flickr.SharedInfo;

public static class PhotoInfo
{
    private const string CanonLensSpec = @"\s?\d{1,3}(-\d{1,3})?\s?mm\sf/\d(\.\d)?(-\d(\.\d))?";
    private const string LensSpec = @"\s?\d{1,3}(-\d{1,3})?\s?mm";
    public static readonly Regex CanonEfLensRegex = new($"^EF{CanonLensSpec}");
    public static readonly Regex CanonEfLLensRegex = new($"^EF{CanonLensSpec}L");
    public static readonly Regex CanonEfMLensRegex = new($"^EF-M{CanonLensSpec}L");
    public static readonly Regex CanonEfSLensRegex = new($"^EF-S{CanonLensSpec}L");
    public static readonly Regex CanonRfLensRegex = new($"^RF{CanonLensSpec}");
    public static readonly Regex CanonRfLLensRegex = new($"^RF{CanonLensSpec}L");
    public static readonly Regex SonyDtLensRegex = new($"DT{LensSpec}");
    public static readonly Regex SonyFeLensRegex = new($"FE{LensSpec}");

    public static readonly Dictionary<Regex, string> CanonLensFamilyRegexes = new()
    {
        [CanonEfLensRegex] = "EF",
        [CanonEfLLensRegex] = "EF L",
        [CanonEfMLensRegex] = "EF-M",
        [CanonEfSLensRegex] = "EF-S",
        [CanonRfLensRegex] = "RF",
        [CanonRfLLensRegex] = "RF L"
    };
}