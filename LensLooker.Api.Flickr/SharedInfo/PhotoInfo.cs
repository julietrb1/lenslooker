using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace LensLooker.Api.Flickr.SharedInfo;

public static class PhotoInfo
{
    private const string CanonLensSpec = @"\s?\d{1,3}(-\d{1,3})?\s?mm\sf/\d(\.\d)?(-\d(\.\d)?)?";
    private const string CanonLensPost = @"(\s|$)";
    private const string SonyLensSpec = @"(\s(PZ)?)?\d{1,3}(-\d{1,3})?\s?mm\sF\d(\.\d)?(-\d(\.\d)?)?";
    private const string SonyGLensPost = @"\sG";
    private const string SonyLensPost = @"($|\s[A-FH-Y])";
    private const string SonyZaLensPost = @"\sZA";

    public static readonly FrozenDictionary<Regex, string> CanonLensFamilyRegexes = new Dictionary<Regex, string>
    {
        [new Regex($"^EF{CanonLensSpec}{CanonLensPost}")] = "EF",
        [new($"^EF{CanonLensSpec}L{CanonLensPost}")] = "EF L",
        [new($"^EF-M{CanonLensSpec}{CanonLensPost}")] = "EF-M",
        [new($"^EF-S{CanonLensSpec}{CanonLensPost}")] = "EF-S",
        [new($"^RF{CanonLensSpec}{CanonLensPost}")] = "RF",
        [new($"^RF{CanonLensSpec}L{CanonLensPost}")] = "RF L"
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<Regex, string> SonyLensFamilyRegexes = new Dictionary<Regex, string>
    {
        [new Regex($"^DT{SonyLensSpec}{SonyLensPost}")] = "DT",
        [new Regex($"^DT{SonyLensSpec}{SonyGLensPost}")] = "DT G",
        [new Regex($"^DT{SonyLensSpec}{SonyZaLensPost}")] = "DT ZA",
        [new Regex($"^E{SonyLensSpec}{SonyLensPost}")] = "E",
        [new Regex($"^FE{SonyLensSpec}{SonyLensPost}")] = "FE",
        [new Regex($"^FE{SonyLensSpec}{SonyGLensPost}")] = "FE G",
        [new Regex($"^FE{SonyLensSpec}{SonyZaLensPost}")] = "FE ZA",
        [new Regex($"^{SonyLensSpec}{SonyLensPost}")] = "SAL",
        [new Regex($"^{SonyLensSpec}{SonyGLensPost}")] = "SAL G",
        [new Regex($"^{SonyLensSpec}{SonyZaLensPost}")] = "SAL ZA"
    }.ToFrozenDictionary();
}
