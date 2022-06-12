using LensLooker.Api.Flickr.Client.Models;

namespace LensLooker.Api.Flickr.Client.GroupsPools.Models;

public record GroupsPoolsGetPhotosRequest(string GroupId) : PaginatedRequest;