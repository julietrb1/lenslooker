namespace LensLooker.Api.Flickr.Config;

public class FlickrOptions
{
    public string ApiKey { get; set; }
    public int MaxRequestsPerHour { get; set; }
    public int MaxPhotosToFetch { get; set; }
    public int PageSize { get; set; }
    public bool FetchOwners { get; set; }
    public IEnumerable<string>? TagsToFetch { get; set; }
    public bool FetchExif { get; set; }
    public IEnumerable<string>? PreferredLenses { get; set; }
    public int? OwnerDeleteThreshold { get; set; }
    public int ExifSaveBatchSize { get; set; }
}