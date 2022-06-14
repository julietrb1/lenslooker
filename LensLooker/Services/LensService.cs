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

    public async Task<bool> TryMatchLensFamilies(Lens lens, Photo photoWithCamera)
    {
        var regexes = GetRegexes(photoWithCamera);
        if (regexes is null)
            return false;

        foreach (var (regex, familyName) in regexes)
        {
            if (!regex.IsMatch(lens.Name)) continue;
            var matchedFamily = await _dbContext.LensFamilies.SingleOrDefaultAsync(f => f.Name == familyName);
            lens.LensFamily = matchedFamily;
            if (matchedFamily != null)
                _logger.LogInformation("Matched lens {Lens} to family {Family}", lens.Name, matchedFamily.Name);

            return matchedFamily != null;
        }

        return false;
    }

    private static Dictionary<Regex, string>? GetRegexes(Photo photoWithCamera)
    {
        switch (photoWithCamera.Camera!.Brand?.Name)
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