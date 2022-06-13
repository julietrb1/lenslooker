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

    // Canon
    public static readonly Regex CanonEfLensRegex = new($"^EF{CanonLensSpec}{CanonLensPost}");
    public static readonly Regex CanonEfLLensRegex = new($"^EF{CanonLensSpec}L{CanonLensPost}");
    public static readonly Regex CanonEfMLensRegex = new($"^EF-M{CanonLensSpec}{CanonLensPost}");
    public static readonly Regex CanonEfSLensRegex = new($"^EF-S{CanonLensSpec}{CanonLensPost}");
    public static readonly Regex CanonRfLensRegex = new($"^RF{CanonLensSpec}{CanonLensPost}");
    public static readonly Regex CanonRfLLensRegex = new($"^RF{CanonLensSpec}L{CanonLensPost}");

    // Sony
    public static readonly Regex SonyDtLensRegex = new($"^DT{SonyLensSpec}{SonyLensPost}");
    public static readonly Regex SonyDtGLensRegex = new($"^DT{SonyLensSpec}{SonyGLensPost}");
    public static readonly Regex SonyDtZaLensRegex = new($"^DT{SonyLensSpec}{SonyZaLensPost}");
    public static readonly Regex SonyELensRegex = new($"^E{SonyLensSpec}{SonyLensPost}");
    public static readonly Regex SonyFeLensRegex = new($"^FE{SonyLensSpec}{SonyLensPost}");
    public static readonly Regex SonyFeGLensRegex = new($"^FE{SonyLensSpec}{SonyGLensPost}");
    public static readonly Regex SonyFeZaLensRegex = new($"^FE{SonyLensSpec}{SonyZaLensPost}");
    public static readonly Regex SonySalLensRegex = new($"^{SonyLensSpec}{SonyLensPost}");
    public static readonly Regex SonySalGLensRegex = new($"^{SonyLensSpec}{SonyGLensPost}");
    public static readonly Regex SonySalZaLensRegex = new($"^{SonyLensSpec}{SonyZaLensPost}");

    public static readonly Dictionary<Regex, string> CanonLensFamilyRegexes = new()
    {
        [CanonEfLensRegex] = "EF",
        [CanonEfLLensRegex] = "EF L",
        [CanonEfMLensRegex] = "EF-M",
        [CanonEfSLensRegex] = "EF-S",
        [CanonRfLensRegex] = "RF",
        [CanonRfLLensRegex] = "RF L"
    };

    public static readonly Dictionary<Regex, string> SonyLensFamilyRegexes = new()
    {
        [SonyDtLensRegex] = "DT",
        [SonyDtGLensRegex] = "DT G",
        [SonyDtZaLensRegex] = "DT ZA",
        [SonyELensRegex] = "E",
        [SonyFeLensRegex] = "FE",
        [SonyFeGLensRegex] = "FE G",
        [SonyFeZaLensRegex] = "FE ZA",
        [SonySalLensRegex] = "SAL",
        [SonySalGLensRegex] = "SAL G",
        [SonySalZaLensRegex] = "SAL ZA"
    };
}