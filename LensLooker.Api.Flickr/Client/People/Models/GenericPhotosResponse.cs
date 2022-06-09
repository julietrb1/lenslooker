using System.Text.Json.Serialization;
using LensLooker.Api.Flickr.Client.Models;

namespace LensLooker.Api.Flickr.Client.People.Models;

public record PhotoResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("owner")] string Owner,
    [property: JsonPropertyName("secret")] string Secret,
    [property: JsonPropertyName("server")] string Server,
    [property: JsonPropertyName("farm")] int Farm,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("ispublic")]
    int IsPublic,
    [property: JsonPropertyName("isfriend")]
    int IsFriend,
    [property: JsonPropertyName("isfamily")]
    int IsFamily
);

public record Photos(
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("pages")] int Pages,
    [property: JsonPropertyName("perpage")]
    int PerPage,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("photo")] IReadOnlyList<PhotoResponse> Photo
);

public record GenericPhotosResponse(
    [property: JsonPropertyName("photos")] Photos Photos
) : GenericResponse;