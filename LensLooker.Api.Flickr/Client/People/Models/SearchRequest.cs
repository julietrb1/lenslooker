using LensLooker.Api.Flickr.Client.Models;

namespace LensLooker.Api.Flickr.Client.People.Models;

public record SearchRequest(IEnumerable<string>? Tags = default) : PaginatedRequest;