namespace LensLooker.Api.Flickr.Client.Photos.Models;

public record GetExifRequest(
    string PhotoId,
    string? Secret = default);