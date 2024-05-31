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
        [new Regex($"^EF{CanonLensSpec}{CanonLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "EF",
        [new($"^EF{CanonLensSpec}L{CanonLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "EF L",
        [new($"^EF-M{CanonLensSpec}{CanonLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "EF-M",
        [new($"^EF-S{CanonLensSpec}{CanonLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "EF-S",
        [new($"^RF{CanonLensSpec}{CanonLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "RF",
        [new($"^RF{CanonLensSpec}L{CanonLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "RF L"
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<Regex, string> SonyLensFamilyRegexes = new Dictionary<Regex, string>
    {
        [new Regex($"^DT{SonyLensSpec}{SonyLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "DT",
        [new Regex($"^DT{SonyLensSpec}{SonyGLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "DT G",
        [new Regex($"^DT{SonyLensSpec}{SonyZaLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "DT ZA",
        [new Regex($"^E{SonyLensSpec}{SonyLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "E",
        [new Regex($"^FE{SonyLensSpec}{SonyLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "FE",
        [new Regex($"^FE{SonyLensSpec}{SonyGLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "FE G",
        [new Regex($"^FE{SonyLensSpec}{SonyZaLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "FE ZA",
        [new Regex($"^{SonyLensSpec}{SonyLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "SAL",
        [new Regex($"^{SonyLensSpec}{SonyGLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "SAL G",
        [new Regex($"^{SonyLensSpec}{SonyZaLensPost}", RegexOptions.None, TimeSpan.FromMilliseconds(100))] = "SAL ZA"
    }.ToFrozenDictionary();
}
