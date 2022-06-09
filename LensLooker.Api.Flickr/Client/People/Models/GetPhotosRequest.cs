using LensLooker.Api.Flickr.Client.Models;

namespace LensLooker.Api.Flickr.Client.People.Models;

public record GetPhotosRequest(
    string UserId,
    string? SafeSearch = default,
    string? MinUploadDate = default,
    string? MaxUploadDate = default,
    string? MinTakenDate = default,
    string? MaxTakenDate = default,
    string? ContentType = default
) : PaginatedRequest;