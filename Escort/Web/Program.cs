using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.IVS;
using Amazon.Ivschat;
using Amazon.Runtime;
using Azure.Storage.Blobs;
using Business.Communication;
using Errlake.Crosscutting;
using IOC.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Shared.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
.AddEnvironmentVariables();

var services = builder.Services;
var configuration = builder.Configuration;

builder.Services.Configure<EmailConfigurationKeys>(configuration.GetSection("EmailConfigurationKeys"));//for Email configuration keys

builder.Services.Configure<SiteKeys>(configuration.GetSection("SiteKeys"));
builder.Services.Configure<AwsKeys>(configuration.GetSection("AwsSettings"));
SiteKeys.SitePhysicalPath = configuration["SiteKeys:SitePhysicalPath"];
SiteKeys.SiteUrl = configuration["SiteKeys:SiteUrl"];
SiteKeys.GeoLocationApiKey = configuration["SiteKeys:GeoLocationApiKey"];
FirebaseKeys.FCMServerKeyFilePath = configuration["FirebaseKeys:FCMServerKeyFilePath"];
FirebaseKeys.FCMProjectId = configuration["FirebaseKeys:FCMProjectId"];
FirebaseKeys.FirebaseMessagingUrl = configuration["FirebaseKeys:FirebaseMessagingUrl"];
SiteKeys.EncryptDecryptKey = builder.Configuration["SiteKeys:EncryptDecryptKey"];
SiteKeys.StripeSecreatKey = builder.Configuration["SiteKeys:StripeSecreatKey"];
SiteKeys.PaypalClientId = builder.Configuration["SiteKeys:PaypalClientId"];
SiteKeys.PaypalClientSecret = builder.Configuration["SiteKeys:PaypalClientSecret"];
SiteKeys.BaseURL = builder.Configuration["SiteKeys:BaseURL"];
SiteKeys.IsLive = Convert.ToBoolean(builder.Configuration["SiteKeys:IsLive"]);
SiteKeys.RememberMeCookieTimeMinutes = Convert.ToInt32(builder.Configuration["SiteKeys:RememberMeCookieTimeMinutes"]);
SiteKeys.AdminPercentage = Convert.ToDecimal(builder.Configuration["SiteKeys:AdminPercentage"]);


services.AddRazorPages().AddRazorRuntimeCompilation();

services.RegisterWebApi(configuration);

// Initialize AWS region (e.g., US West 2)
var region = RegionEndpoint.APSoutheast2; // Replace with your desired region

// Set the region globally for all AWS SDK clients
AWSConfigs.AWSRegion = region.SystemName;
services.AddAWSService<IAmazonIVS>();
services.AddAWSService<IAmazonIvschat>();

services.AddAuthorization(config =>
{
    config.AddPolicy("AuthorisedUser", policy =>
    {
        policy.RequireClaim("UserId");
    });

    config.AddPolicy("AdminRolePolicy", policy =>
    {
        policy.RequireClaim("Device");
        policy.RequireClaim("DeviceType");
        policy.RequireClaim("Offset");
    });

});

services.AddAuthentication("CookiesAuth").AddCookie("CookiesAuth", config =>
{
    config.Cookie.Name = "Identitye.Cookie";
    config.LoginPath = "/Account/login";
});

services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization();

services.AddControllersWithViews();
services.AddError();
services.AddSession();
services.AddHttpClient();

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.ValueCountLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
    x.MultipartHeadersLengthLimit = int.MaxValue;
    x.MemoryBufferThreshold = int.MaxValue;

});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.ConfigureExceptionMiddlewareHandler();


app.UseHttpsRedirection();
app.UseStatusCodePagesWithRedirects("/Error/Error{0}");
app.UseAuthentication();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.UseCookiePolicy();
app.MapControllers();
app.MapRazorPages();
app.MapControllerRoute("areaRoute", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

