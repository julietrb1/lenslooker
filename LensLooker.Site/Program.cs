using LensLooker.Data;
using LensLooker.Site.Config;
using LensLooker.Site.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

if (!builder.Environment.IsDevelopment())
    builder.WebHost.UseSentry(o => { o.TracesSampleRate = 1.0; });

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.Configure<SiteOptions>(builder.Configuration.GetRequiredSection(nameof(SiteOptions)));

builder.Services.AddDbContext<LensLookerContext>(b =>
    b.UseSqlServer(builder.Configuration.GetConnectionString("LensLookerContext")));

builder.Services.AddScoped<IPhotoService, PhotoService>();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<LensLookerContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        var previousExecutionTimeout = context.Database.GetCommandTimeout();
        context.Database.SetCommandTimeout(180);
        context.Database.Migrate();
        context.Database.SetCommandTimeout(previousExecutionTimeout);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

if (!app.Environment.IsDevelopment())
    app.UseSentryTracing();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();