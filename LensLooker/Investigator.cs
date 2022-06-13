using System.Text.RegularExpressions;
using LensLooker.Api.Flickr.Client.Common;
using LensLooker.Api.Flickr.Client.GroupsPools;
using LensLooker.Api.Flickr.Client.GroupsPools.Models;
using LensLooker.Api.Flickr.Client.People;
using LensLooker.Api.Flickr.Client.People.Models;
using LensLooker.Api.Flickr.Client.Photos;
using LensLooker.Api.Flickr.Client.Photos.Models;
using LensLooker.Api.Flickr.Config;
using LensLooker.Api.Flickr.Exceptions;
using LensLooker.Api.Flickr.SharedInfo;
using LensLooker.Data;
using LensLooker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LensLooker;

public class Investigator : IInvestigator
{
    private readonly LensLookerContext _dbContext;
    private readonly IGroupsPoolsClient _groupsPoolsClient;
    private readonly ILogger<Investigator> _logger;
    private readonly FlickrOptions _options;
    private readonly IPeopleClient _peopleClient;
    private readonly IPhotosClient _photosClient;

    public Investigator(IPhotosClient photosClient, IPeopleClient peopleClient, IGroupsPoolsClient groupsPoolsClient,
        ILogger<Investigator> logger,
        LensLookerContext dbContext, IOptions<FlickrOptions> options)
    {
        _photosClient = photosClient;
        _peopleClient = peopleClient;
        _groupsPoolsClient = groupsPoolsClient;
        _logger = logger;
        _dbContext = dbContext;
        _options = options.Value;
    }

    public async Task Investigate()
    {
        await OrganiseCamerasAndLenses();

        if (_options.TagsToFetch?.Any() == true) await FetchPhotosWithTags();

        if (_options.GroupsToFetch?.Any() == true) await FetchGroups();

        if (_options.PreferredLenses?.Any() == true) await FetchPreferredLenses();

        if (_options.FetchOwners) await FetchOwners();

        if (_options.FetchExif)
        {
            if (_options.OwnerDeleteThreshold.HasValue)
            {
                await SetNoLensToSkipped();
                await _dbContext.SaveChangesAsync();
            }

            // Changes saved within, so no need to save changes on context here.
            await FetchPhotoExif();
        }
    }

    private async Task OrganiseCamerasAndLenses()
    {
        var brands = await _dbContext.Brands.ToListAsync();
        var camerasWithoutBrand = await _dbContext.Cameras
            .Where(c => c.Brand == null)
            .ToListAsync();

        foreach (var camera in camerasWithoutBrand)
        {
            var matchingBrand =
                brands.SingleOrDefault(b => camera.Name.ToLowerInvariant().StartsWith(b.Name.ToLowerInvariant()));
            if (matchingBrand == null)
            {
                _logger.LogWarning("No brand found to assign to camera {Camera}", camera.Name);
                continue;
            }

            _logger.LogInformation("Assigning camera {Camera} to brand {Brand}", camera.Name, matchingBrand.Name);
            camera.Brand = matchingBrand;
        }

        await _dbContext.SaveChangesAsync();

        var lensWithoutBrand = await _dbContext.Lenses.Where(l => l.Brand == null).ToListAsync();

        foreach (var lens in lensWithoutBrand)
        {
            var photoWithCamera = lens.Photos.FirstOrDefault(p => p.Camera != null);
            if (photoWithCamera == null)
            {
                _logger.LogWarning("Lens {Lens} has no matching photo to infer brand from camera", lens.Name);
                continue;
            }

            _logger.LogWarning("Lens {Lens} has no brand (camera {Camera})", lens.Name,
                photoWithCamera.Camera?.Name);
        }

        foreach (var lens in _dbContext.Lenses)
        {
            var photoWithCamera = lens.Photos.FirstOrDefault(p => p.Camera != null);
            if (photoWithCamera == null)
            {
                _logger.LogWarning("Lens {Lens} has no matching photo to infer family from camera", lens.Name);
                continue;
            }

            if (!await TryMatchCanonLensFamilies(lens, photoWithCamera) &&
                !await TryMatchSonyLensFamilies(lens, photoWithCamera))
                _logger.LogWarning("Lens {Lens} family unmatched (from {CameraBrand} camera)", lens.Name,
                    photoWithCamera.Camera!.Brand?.Name);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<bool> TryMatchCanonLensFamilies(Lens lens, Photo photoWithCamera)
    {
        return await TryMatchLensFamilies(lens, photoWithCamera, PhotoInfo.CanonLensFamilyRegexes, "Canon");
    }

    private async Task<bool> TryMatchSonyLensFamilies(Lens lens, Photo photoWithCamera)
    {
        return await TryMatchLensFamilies(lens, photoWithCamera, PhotoInfo.SonyLensFamilyRegexes, "Sony");
    }

    private async Task<bool> TryMatchLensFamilies(Lens lens, Photo photoWithCamera,
        Dictionary<Regex, string> canonLensFamilyRegexes, string brandName)
    {
        if (photoWithCamera.Camera!.Brand?.Name != brandName)
            return false;

        foreach (var (regex, familyName) in canonLensFamilyRegexes)
        {
            if (!regex.IsMatch(lens.Name)) continue;
            var matchedFamily = await _dbContext.LensFamilies.SingleOrDefaultAsync(f => f.Name == familyName);
            lens.LensFamily = matchedFamily;
            _logger.LogInformation("Matched lens {Lens} to family {Family}", lens.Name, matchedFamily?.Name);
            return true;
        }

        return false;
    }

    private async Task FetchPhotosWithTags()
    {
        _logger.LogInformation("Fetching tags (currently {} photos)",
            await GetCurrentCountString());
        await RefreshPhotosFromFlickr(photosPage => _photosClient.Search(new SearchRequest
            { Page = photosPage, PerPage = _options.PageSize, Tags = _options.TagsToFetch }));
        _logger.LogInformation("Tags fetched (currently {} photos)",
            await GetCurrentCountString());
        await _dbContext.SaveChangesAsync();
    }

    private async Task FetchGroups()
    {
        _logger.LogInformation("Fetching {GroupCount} groups (currently {CurrentCount} photos)",
            _options.GroupsToFetch!.Count().ToString("N0"),
            await GetCurrentCountString());
        foreach (var groupId in _options.GroupsToFetch!)
        {
            _logger.LogInformation("Fetching group {GroupId} (currently {CurrentCount} photos)", groupId,
                await GetCurrentCountString());
            await RefreshPhotosFromFlickr(photosPage => _groupsPoolsClient.GetPhotos(
                new GroupsPoolsGetPhotosRequest(groupId)
                    { Page = photosPage, PerPage = _options.PageSize }));
            _logger.LogInformation("Finished fetching group {GroupId} (currently {CurrentCount} photos)", groupId,
                await _dbContext.Photos.CountAsync());
        }

        _logger.LogInformation("Groups fetched (currently {} photos)",
            await GetCurrentCountString());
        await _dbContext.SaveChangesAsync();
    }

    private async Task<string> GetCurrentCountString()
    {
        return (await _dbContext.Photos.CountAsync()).ToString("N0");
    }

    private async Task FetchOwners()
    {
        _logger.LogInformation("DB has {} photos before fetching owners",
            await GetCurrentCountString());
        var ownerIds = await _dbContext.Photos.Select(p => p.OwnerId).GroupBy(o => o).Select(g => g.Key)
            .ToListAsync();
        foreach (var ownerId in ownerIds) await FetchOwner(ownerId);

        _logger.LogInformation("Database has {} photos after fetching owners",
            await GetCurrentCountString());
    }

    private async Task FetchOwner(string ownerId)
    {
        _logger.LogInformation("Fetching photos for owner {}", ownerId);
        await RefreshPhotosFromFlickr(photosPage => _peopleClient.GetPublicPhotos(
            new GetPublicPhotosRequest(ownerId)
                { Page = photosPage, PerPage = _options.PageSize }), true);
        // It's safe to save here, since the foreach loop has been ToListed, meaning that a nested SQL query
        // won't be made
        await _dbContext.SaveChangesAsync();
    }

    private async Task FetchPreferredLenses()
    {
        foreach (var lensName in _options.PreferredLenses!)
        {
            var lens = await _dbContext.Lenses.FindAsync(lensName);
            if (lens == null)
            {
                _logger.LogWarning("No preferred lens '{}' found", lensName);
                continue;
            }

            _logger.LogInformation("Fetching photos for owners of lens '{}'", lensName);

            foreach (var ownerGroup in lens.Photos.GroupBy(p => p.OwnerId))
            {
                _logger.LogInformation("Fetching photos of user '{OwnerId}' (for lens '{LensName}')",
                    ownerGroup.Key, lensName);
                await RefreshPhotosFromFlickr(photosPage =>
                    _peopleClient.GetPublicPhotos(new GetPublicPhotosRequest(ownerGroup.Key)
                        { Page = photosPage, PerPage = _options.PageSize }), true);
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task SetNoLensToSkipped()
    {
        var photosWithExifNoLens = _dbContext.Photos.Where(p => !p.IsSkipped && p.IsExifFetched && p.Lens == null);
        if (!photosWithExifNoLens.Any())
            return;

        _logger.LogWarning("Setting {NewSkipCount} photos with no lens as skipped",
            await photosWithExifNoLens.CountAsync());

        foreach (var photo in photosWithExifNoLens)
            photo.IsSkipped = true;
    }

    private async Task RefreshPhotosFromFlickr(Func<int, Task<GenericPhotosResponse>> searchFunction,
        bool ignorePhotoLimit = false)
    {
        var photosPage = 1;
        GenericPhotosResponse? photosResponse;
        var fetchedPhotos = 0;
        int? lastPageCount = null;
        do
        {
            _logger.LogInformation("Fetching page {Page}/{PageCount}", photosPage.ToString("N0"),
                lastPageCount?.ToString("N0") ?? "?");
            photosResponse = await searchFunction(photosPage);
            lastPageCount = photosResponse.Photos.Pages;

            await SavePhotos(photosResponse);

            fetchedPhotos += photosResponse.Photos.Photo.Count;
            _logger.LogInformation("Got {ResponseCount} more photos ({TotalPhotos} total)",
                photosResponse.Photos.Photo.Count.ToString("N0"), fetchedPhotos.ToString("N0"));
            await _dbContext.SaveChangesAsync();
        } while ((ignorePhotoLimit || fetchedPhotos < _options.MaxPhotosToFetch) &&
                 photosResponse.Photos.Pages > photosPage++);
    }

    private async Task FetchPhotoExif()
    {
        _logger.LogInformation("Fetching missing EXIF data for all photos");
        var photosWithoutExif = await GetPhotosWithoutExif();
        var ownerStrikeCountMap = new Dictionary<string, int>();
        do
        {
            foreach (var photo in photosWithoutExif)
            {
                if (_options.OwnerDeleteThreshold.HasValue &&
                    ownerStrikeCountMap.TryGetValue(photo.OwnerId, out var initialOwnerPhotoCount) &&
                    initialOwnerPhotoCount >= _options.OwnerDeleteThreshold)
                {
                    _logger.LogWarning(
                        "Skipping photo '{PhotoId}' by '{OwnerId}' because of strike count {BadPhotoCount}",
                        photo.PhotoId, photo.OwnerId, ownerStrikeCountMap[photo.OwnerId]);
                    photo.IsSkipped = true;
                    // No save here because doing so in succession within the loop would be inefficient.
                    continue;
                }

                photo.IsExifFetched = true;
                GetExifResponse? fetchedPhoto;
                try
                {
                    fetchedPhoto = await _photosClient.GetExif(new GetExifRequest(photo.PhotoId));
                }
                catch (PhotoNotFoundException)
                {
                    _logger.LogWarning("Photo '{PhotoId}' not be found on Flickr",
                        photo.PhotoId);
                    continue;
                }
                catch (PermissionDeniedException)
                {
                    if (_options.OwnerDeleteThreshold.HasValue)
                    {
                        ownerStrikeCountMap.TryGetValue(photo.OwnerId, out var ownerPhotoCount);
                        ownerStrikeCountMap[photo.OwnerId] = ownerPhotoCount + 1;
                        _logger.LogWarning(
                            "At {Strikes}/{TotalStrikes} strikes for owner because of EXIF permission denied for photo '{PhotoId}'",
                            ownerStrikeCountMap[photo.OwnerId], _options.OwnerDeleteThreshold, photo.PhotoId);
                    }
                    else
                    {
                        _logger.LogWarning("Permission denied for EXIF of '{PhotoId}'", photo.PhotoId);
                    }

                    continue;
                }

                var fetchedLens = fetchedPhoto.Photo.Exif.FirstOrDefault(e => e.Tag == "LensModel")?.Raw.Content
                    .ToString();
                var lens = string.IsNullOrWhiteSpace(fetchedLens)
                    ? null
                    : await _dbContext.Lenses.FindAsync(fetchedLens) ??
                      new Lens { Name = fetchedLens };
                photo.Lens = lens;

                if (_options.OwnerDeleteThreshold.HasValue && photo.Lens == null)
                {
                    ownerStrikeCountMap.TryGetValue(photo.OwnerId, out var ownerPhotoCount);
                    ownerStrikeCountMap[photo.OwnerId] = ownerPhotoCount + 1;
                    _logger.LogWarning(
                        "At {BadPhotoCount}/{TriggerPhotoCount} strikes for owner '{OwnerId}' because of lens '{LensName}'",
                        ownerStrikeCountMap[photo.OwnerId], _options.OwnerDeleteThreshold, photo.OwnerId,
                        photo.Lens?.Name);
                    if (ownerStrikeCountMap[photo.OwnerId] >= _options.OwnerDeleteThreshold)
                    {
                        _logger.LogWarning(
                            "Skipping photo '{PhotoId}' from '{OwnerId}' because of lens '{LensName}' at count {PhotoCount}",
                            photo.PhotoId, photo.OwnerId, photo.Lens?.Name, ownerPhotoCount);
                        photo.IsSkipped = true;
                        continue;
                    }
                }

                var rawDate = fetchedPhoto.Photo.Exif.FirstOrDefault(e => e.Tag == "DateTimeOriginal")?.Raw.Content
                    .ToString();
                DateTime? parsedDate =
                    rawDate == null ? null : DateTime.ParseExact(rawDate, "yyyy:MM:dd HH:mm:ss", null);
                photo.DateTimeShot = parsedDate;

                var fetchedCamera = fetchedPhoto.Photo.Camera;
                var camera = string.IsNullOrWhiteSpace(fetchedCamera)
                    ? null
                    : await _dbContext.Cameras.FindAsync(fetchedCamera) ??
                      new Camera { Name = fetchedCamera };
                photo.Camera = camera;

                var focalLengthNode = fetchedPhoto.Photo.Exif.FirstOrDefault(e => e.Tag == "FocalLength");
                var fetchedFocalLength =
                    focalLengthNode?.Raw.Content.Split().FirstOrDefault();
                int? focalLength = double.TryParse(fetchedFocalLength, out var parsedFocalLength)
                    ? (int)Math.Round(parsedFocalLength)
                    : null;
                photo.FocalLengthInMm = focalLength;

                var fNumberNode = fetchedPhoto.Photo.Exif.FirstOrDefault(e => e.Tag == "FNumber");
                photo.FNumber = double.TryParse(fNumberNode?.Raw.Content, out var parsedFNumber)
                    ? parsedFNumber
                    : null;

                var exposureTimeNode = fetchedPhoto.Photo.Exif.FirstOrDefault(e => e.Tag == "ExposureTime");
                var fetchedExposureTime =
                    exposureTimeNode?.Clean?.Content;
                photo.ExposureTime = fetchedExposureTime;

                var isoNode = fetchedPhoto.Photo.Exif.FirstOrDefault(e => e.Tag == "ISO");
                photo.Iso = int.TryParse(isoNode?.Raw.Content, out var parsedIso) ? parsedIso : null;

                _logger.LogInformation(
                    "Saving photo {PhotoId}, camera '{Camera}', lens '{Lens}', '{FocalLength} mm', 'f/{Aperture}', ISO '{Iso}', date '{DateTimeShot}'",
                    photo.PhotoId, photo.Camera?.Name,
                    photo.Lens?.Name, photo.FocalLengthInMm, photo.FNumber, photo.Iso?.ToString("N0"),
                    photo.DateTimeShot);

                // Need to save at this point so the related Camera and Lens models are created.
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.SaveChangesAsync();
            photosWithoutExif = await GetPhotosWithoutExif();
        } while (photosWithoutExif.Any());
    }

    private Task<List<Photo>> GetPhotosWithoutExif()
    {
        return _dbContext.Photos
            .Where(p => !p.IsExifFetched && !p.IsSkipped)
            .OrderBy(p => p.PhotoId)
            .Take(_options.ExifSaveBatchSize)
            .ToListAsync();
    }

    private async Task SavePhotos(GenericPhotosResponse photosResponse)
    {
        foreach (var photo in photosResponse.Photos.Photo)
        {
            var matchingRecord = await _dbContext.Photos.FindAsync(photo.Id);
            if (matchingRecord == null)
            {
                await _dbContext.Photos.AddAsync(
                    new Photo
                    {
                        PhotoId = photo.Id, OwnerId = photo.Owner, Title = photo.Title, Farm = photo.Farm,
                        Server = photo.Server, Secret = photo.Secret
                    });
            }
            else
            {
                matchingRecord.OwnerId = photo.Owner;
                matchingRecord.Title = photo.Title;
                matchingRecord.Farm = photo.Farm;
                matchingRecord.Server = photo.Server;
                matchingRecord.Secret = photo.Secret;
            }
        }
    }
}