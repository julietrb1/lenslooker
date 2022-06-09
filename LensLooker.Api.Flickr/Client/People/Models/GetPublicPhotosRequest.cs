using LensLooker.Api.Flickr.Client.Models;

namespace LensLooker.Api.Flickr.Client.People.Models;

public record GetPublicPhotosRequest(
    string UserId,
    string? SafeSearch = default,
    IEnumerable<string>? Extras = default
) : PaginatedRequest;