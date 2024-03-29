﻿using LensLooker;
using LensLooker.Api.Flickr.Client.GroupsPools;
using LensLooker.Api.Flickr.Client.People;
using LensLooker.Api.Flickr.Client.Photos;
using LensLooker.Api.Flickr.Config;
using LensLooker.Data;
using LensLooker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Sentry;

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
            retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetRateLimitPolicy()
{
    return Policy
        .RateLimitAsync<HttpResponseMessage>(3600, TimeSpan.FromHours(1), 4);
}

IHost BuildHost()
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((_, builder) =>
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();
        })
        .ConfigureServices((context, services) =>
        {
            services
                .AddOptions()
                .AddDbContext<LensLookerContext>(builder =>
                    builder
                        .UseLazyLoadingProxies()
                        .UseSqlServer(context.Configuration.GetConnectionString("LensLookerContext"))
                )
                .Configure<FlickrOptions>(context.Configuration.GetRequiredSection(nameof(FlickrOptions)))
                .AddTransient<IInvestigator, Investigator>()
                .AddTransient<ILensService, LensService>();

            services
                .AddHttpClient<IPhotosClient, PhotosClient>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetRateLimitPolicy());

            services
                .AddHttpClient<IPeopleClient, PeopleClient>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetRateLimitPolicy());

            services
                .AddHttpClient<IGroupsPoolsClient, GroupsPoolsClient>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetRateLimitPolicy());
        })
        .Build();
}

using var sdk = SentrySdk.Init(o => { o.TracesSampleRate = 1.0; });

try
{
    var host = BuildHost();
    Console.WriteLine("Starting Focal Investigator.");
    await host.Services.GetRequiredService<IInvestigator>().Investigate();
}
catch (Exception e)
{
    SentrySdk.CaptureException(e);
    await SentrySdk.FlushAsync(TimeSpan.FromSeconds(2));
    throw;
}