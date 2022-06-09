using System.Text.Json.Serialization;
using LensLooker.Api.Flickr.Client.Models;

namespace LensLooker.Api.Flickr.Client.Photos.Models;

public record Clean(
    [property: JsonPropertyName("_content")]
    string Content
);

public record ResponseExif(
    [property: JsonPropertyName("tagspace")]
    string TagSpace,
    [property: JsonPropertyName("tagspaceid")]
    int TagSpaceId,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("raw")] Raw Raw,
    [property: JsonPropertyName("clean")] Clean? Clean
);

public record PhotoExifInfo(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("secret")] string Secret,
    [property: JsonPropertyName("server")] string Server,
    [property: JsonPropertyName("farm")] int Farm,
    [property: JsonPropertyName("camera")] string Camera,
    [property: JsonPropertyName("exif")] IReadOnlyList<ResponseExif> Exif
);

public record Raw(
    [property: JsonPropertyName("_content")]
    string Content
);

public record GetExifResponse(
    [property: JsonPropertyName("photo")] PhotoExifInfo Photo
) : GenericResponse;