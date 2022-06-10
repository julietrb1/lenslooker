using LensLooker.Api.Flickr.Client.People;
using LensLooker.Api.Flickr.Client.People.Models;
using LensLooker.Api.Flickr.Client.Photos;
using LensLooker.Api.Flickr.Client.Photos.Models;
using LensLooker.Api.Flickr.Config;
using LensLooker.Api.Flickr.Exceptions;
using LensLooker.Data;
using LensLooker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LensLooker;

public class Investigator : IInvestigator
{
    private readonly LensLookerContext _dbContext;
    private readonly ILogger<Investigator> _logger;
    private readonly FlickrOptions _options;
    private readonly IPeopleClient _peopleClient;
    private readonly IPhotosClient _photosClient;

    public Investigator(IPhotosClient photosClient, IPeopleClient peopleClient, ILogger<Investigator> logger,
        LensLookerContext dbContext, IOptions<FlickrOptions> options)
    {
        _photosClient = photosClient;
        _peopleClient = peopleClient;
        _logger = logger;
        _dbContext = dbContext;
        _options = options.Value;
    }

    public async Task Investigate()
    {
        if (_options.TagsToFetch?.Any() == true)
        {
            _logger.LogInformation("Fetching tags (currently {} photos)",
                (await _dbContext.Photos.CountAsync()).ToString("N0"));
            await RefreshPhotosFromFlickr(photosPage => _photosClient.Search(new SearchRequest
                { Page = photosPage, PerPage = _options.PageSize, Tags = _options.TagsToFetch }));
            _logger.LogInformation("Tags fetched (currently {} photos)",
                (await _dbContext.Photos.CountAsync()).ToString("N0"));
            await _dbContext.SaveChangesAsync();
        }

        if (_options.PreferredLenses?.Any() == true)
            foreach (var lensName in _options.PreferredLenses)
            {
                var lens = await _dbContext.Lenses.FindAsync(lensName);
                if (lens == null)
                {
                    _logger.LogWarning("No preferred lens '{}' found", lensName);
                    continue;
                }

                _logger.LogInformation("Fetching photos for owners of lens '{}'", lensName);
                await _dbContext.Entry(lens).Collection(l => l.Photos).LoadAsync();

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

        if (_options.FetchOwners)
        {
            _logger.LogInformation("DB has {} photos before fetching owners",
                (await _dbContext.Photos.CountAsync()).ToString("N0"));
            var ownerIds = await _dbContext.Photos.Select(p => p.OwnerId).GroupBy(o => o).Select(g => g.Key)
                .ToListAsync();
            foreach (var ownerId in ownerIds)
            {
                _logger.LogInformation("Fetching photos for owner {}", ownerId);
                await RefreshPhotosFromFlickr(photosPage => _peopleClient.GetPublicPhotos(
                    new GetPublicPhotosRequest(ownerId)
                        { Page = photosPage, PerPage = _options.PageSize }), true);
                // It's safe to save here, since the foreach loop has been ToListed, meaning that a nested SQL query
                // won't be made
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("DB has {} photos after fetching owners",
                (await _dbContext.Photos.CountAsync()).ToString("N0"));
        }

        if (_options.FetchExif)
        {
            if (_options.OwnerDeleteThreshold.HasValue)
            {
                await SetNoLensToSkipped();
                await _dbContext.SaveChangesAsync();
            }

            // Changes saved within, so no need to save changes on context here.
            await SavePhotoExif();
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

    private async Task SavePhotoExif()
    {
        _logger.LogInformation("Fetching missing EXIF data for all photos");
        var photosWithoutExif = await GetPhotosWithoutExif();
        var ownerCountMap = new Dictionary<string, int>();
        do
        {
            foreach (var photo in photosWithoutExif)
            {
                if (_options.OwnerDeleteThreshold.HasValue &&
                    ownerCountMap.TryGetValue(photo.OwnerId, out var initialOwnerPhotoCount) &&
                    initialOwnerPhotoCount >= _options.OwnerDeleteThreshold)
                {
                    _logger.LogWarning(
                        "Skipping photo '{PhotoId}' by '{OwnerId}' because of bad photo count {BadPhotoCount}",
                        photo.PhotoId, photo.OwnerId, ownerCountMap[photo.OwnerId]);
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
                    _logger.LogWarning("Permission denied for EXIF of '{PhotoId}'",
                        photo.PhotoId);
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
                    ownerCountMap.TryGetValue(photo.OwnerId, out var ownerPhotoCount);
                    ownerCountMap[photo.OwnerId] = ownerPhotoCount + 1;
                    _logger.LogWarning(
                        "At {BadPhotoCount}/{TriggerPhotoCount} photos for owner '{OwnerId}' because of lens '{LensName}'",
                        ownerCountMap[photo.OwnerId], _options.OwnerDeleteThreshold, photo.OwnerId, photo.Lens?.Name);
                    if (ownerCountMap[photo.OwnerId] >= _options.OwnerDeleteThreshold)
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

                await _dbContext.SaveChangesAsync();
            }

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