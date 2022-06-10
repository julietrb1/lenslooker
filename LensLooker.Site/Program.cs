using LensLooker.Data;
using LensLooker.Site.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.WebHost.UseSentry(o => { o.TracesSampleRate = 1.0; });

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddDbContext<LensLookerContext>(b =>
    b.UseSqlServer(builder.Configuration.GetConnectionString("LensLookerContext")));

var app = builder.Build();

var context = app.Services.GetRequiredService<LensLookerContext>();
if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();

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
app.UseSentryTracing();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();