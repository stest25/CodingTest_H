using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using VideoMediaApp.Services;
using VideoMediaApp.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure MediaFileOptions from appsettings.json
builder.Services.Configure<MediaFileOptions>(
    builder.Configuration.GetSection("MediaFileOptions"));

// Register services
builder.Services.AddScoped<IMediaFileService, MediaFileService>();
builder.Services.AddScoped<IVideoFileMapper, VideoFileMapper>();
builder.Services.AddScoped<IViewModelFactory, ViewModelFactory>();

// Configure maximum upload size for forms - Use value from MediaFileOptions
var mediaOptions = builder.Configuration
    .GetSection("MediaFileOptions")
    .Get<MediaFileOptions>() ?? new MediaFileOptions();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = mediaOptions.MaxFileSizeBytes;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
  name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
