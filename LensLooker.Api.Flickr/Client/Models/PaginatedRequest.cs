namespace LensLooker.Api.Flickr.Client.Models;

public record PaginatedRequest(
    int Page = 1,
    int PerPage = 100
);