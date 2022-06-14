using LensLooker.Data.Models;

namespace LensLooker.Services;

public interface ILensService
{
    Task<bool> TryMatchLensFamilies(Lens lens, Photo photoWithCamera);
}