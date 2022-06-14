using System.Text.RegularExpressions;
using LensLooker.Api.Flickr.SharedInfo;
using LensLooker.Data;
using LensLooker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LensLooker.Services;

public class LensService : ILensService
{
    private readonly LensLookerContext _dbContext;
    private readonly ILogger<LensService> _logger;

    public LensService(LensLookerContext dbContext, ILogger<LensService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<LensFamily?> MatchLensFamily(string lensName, string cameraBrandName)
    {
        var regexes = GetRegexes(cameraBrandName);
        if (regexes is null)
            return null;

        foreach (var (regex, familyName) in regexes)
        {
            if (!regex.IsMatch(lensName)) continue;
            var matchedFamily = await _dbContext.LensFamilies.SingleOrDefaultAsync(f => f.Name == familyName);
            if (matchedFamily != null)
            {
                _logger.LogInformation("Matched lens {Lens} to family {Family}", lensName, matchedFamily.Name);
                return matchedFamily;
            }

            return null;
        }

        return null;
    }

    private static Dictionary<Regex, string>? GetRegexes(string cameraBrandName)
    {
        switch (cameraBrandName)
        {
            case "Canon":
                return PhotoInfo.CanonLensFamilyRegexes;
            case "Sony":
                return PhotoInfo.SonyLensFamilyRegexes;
            default:
                return null;
        }
    }
}