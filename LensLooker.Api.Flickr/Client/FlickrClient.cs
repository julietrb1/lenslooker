using System.Net.Http.Json;
using System.Text.Json;
using LensLooker.Api.Flickr.Client.Models;
using LensLooker.Api.Flickr.Config;
using LensLooker.Api.Flickr.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.RateLimit;

namespace LensLooker.Api.Flickr.Client;

public class FlickrClient : IFlickrClient
{
    private const string FlickrStatFail = "fail";
    private readonly HttpClient _httpClient;
    private readonly ILogger<FlickrClient> _logger;
    private readonly int _secondsBetweenRequests;
    private readonly SemaphoreSlim _semaphoreSlim = new(1);
    private DateTime? _lastRequestAt;

    protected FlickrClient(IOptions<FlickrOptions> options, HttpClient httpClient, ILogger<FlickrClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://www.flickr.com/services/rest/");
        ApiKey = options.Value.ApiKey;
        _secondsBetweenRequests = options.Value.MaxRequestsPerHour / 3600;
        _logger = logger;
    }

    private string ApiKey { get; }

    protected async Task<TReturn> GetWithRateLimiting<TReturn>(string flickrMethod, string? queryParams = default)
        where TReturn : GenericResponse
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get,
            $"?api_key={ApiKey}&nojsoncallback=1&format=json&method={flickrMethod}&{queryParams}");
        var response = await SendRateLimitedRequest(requestMessage);
        try
        {
            var parsedResponse = await response.Content.ReadFromJsonAsync<TReturn>() ??
                                 throw new InvalidOperationException(
                                     $"Got an invalid response when fetching from server with method {flickrMethod}.");

            if (parsedResponse.Stat == FlickrStatFail)
                throw FlickrException.GenerateException(parsedResponse.Stat, parsedResponse.Code,
                    parsedResponse.Message);

            return parsedResponse;
        }
        catch (JsonException e)
        {
            _logger.LogError("Failed to parse JSON: {}\nMessage: {}", e, await response.Content.ReadAsStringAsync());
            throw;
        }
    }

    private async Task<HttpResponseMessage> SendRateLimitedRequest(HttpRequestMessage requestMessage)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            var nextRequestIn = _lastRequestAt == null
                ? TimeSpan.Zero
                : (_lastRequestAt + TimeSpan.FromSeconds(_secondsBetweenRequests) - DateTime.Now).Value;
            if (nextRequestIn > TimeSpan.Zero)
            {
                _logger.LogDebug("Waiting {} before sending request", nextRequestIn);
                Thread.Sleep(nextRequestIn);
            }

            while (true)
                try
                {
                    return (await _httpClient.SendAsync(requestMessage)).EnsureSuccessStatusCode();
                }
                catch (RateLimitRejectedException)
                {
                    const int sleepSeconds = 10;
                    _logger.LogWarning("Sleeping for {} seconds as Flickr request rate limited", sleepSeconds);
                    Thread.Sleep(TimeSpan.FromSeconds(sleepSeconds));
                }
                finally
                {
                    _lastRequestAt = DateTime.Now;
                }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}