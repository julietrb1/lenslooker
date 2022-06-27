using LensLooker.Data.Models;

namespace LensLooker.Site.Data;

public static class ModelExtensions
{
    public enum PhotoSize
    {
        ThumbnailCroppedSquare75,
        ThumbnailCroppedSquare150,
        Thumbnail100,
        Small240,
        Small320,
        Small400,
        Medium500,
        Medium640,
        Medium800,
        Large1024,
        Large1600,
        Large2048,
        ExtraLarge3072,
        ExtraLarge4096,
        ExtraLarge4096TwoToOne,
        ExtraLarge5120,
        ExtraLarge6144,
        Original
    }

    public static PhotoDto ToPhotoDto(this Photo photo, PhotoSize size)
    {
        var suffix = GetPhotoSuffix(size);
        var srcUrl = $"https://live.staticflickr.com/{photo.Server}/{photo.PhotoId}_{photo.Secret}{suffix}.jpg";
        var viewingUrl = $"https://www.flickr.com/photos/{photo.OwnerId}/{photo.PhotoId}/";
        return new PhotoDto(
            srcUrl, viewingUrl, photo.OwnerId, photo.Title,
            photo.Camera?.Name, photo.Lens?.Name, photo.FocalLengthInMm, photo.FNumber,
            photo.ExposureTime);
    }

    private static string GetPhotoSuffix(PhotoSize size)
    {
        return size switch
        {
            PhotoSize.ThumbnailCroppedSquare75 => "_s",
            PhotoSize.ThumbnailCroppedSquare150 => "_q",
            PhotoSize.Thumbnail100 => "_t",
            PhotoSize.Small240 => "_m",
            PhotoSize.Small320 => "_n",
            PhotoSize.Small400 => "_w",
            PhotoSize.Medium640 => "_z",
            PhotoSize.Medium800 => "_c",
            PhotoSize.Large1024 => "_b",
            PhotoSize.Large1600 => "_h",
            PhotoSize.Large2048 => "_k",
            PhotoSize.ExtraLarge3072 => "_3k",
            PhotoSize.ExtraLarge4096 => "_4k",
            PhotoSize.ExtraLarge4096TwoToOne => "_f",
            PhotoSize.ExtraLarge5120 => "_5k",
            PhotoSize.ExtraLarge6144 => "_6k",
            PhotoSize.Original => "_o",
            _ => string.Empty
        };
    }
}