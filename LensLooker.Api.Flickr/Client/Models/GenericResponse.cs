using System.Text.Json.Serialization;

namespace LensLooker.Api.Flickr.Client.Models;

public record GenericResponse
{
    [property: JsonPropertyName("stat")] public string Stat { get; init; }
    [property: JsonPropertyName("code")] public int? Code { get; init; } = default!;

    [property: JsonPropertyName("message")]
    public string? Message { get; init; } = default!;
}