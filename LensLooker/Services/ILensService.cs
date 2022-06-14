using LensLooker.Data.Models;

namespace LensLooker.Services;

public interface ILensService
{
    Task<LensFamily?> MatchLensFamily(string lensName, string cameraBrandName);
}