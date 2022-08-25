
using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Npgsql;
using video_streamming_proxy.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddScoped<IDbConnection>((sp) => new NpgsqlConnection("Server=database;Port=5432;Database=vstream;User Id=postgres;Password=postgres"));
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.AccessDeniedPath = "/denied";
        options.LoginPath = "/Home";
        options.Cookie.Name = "vstream";

    });
    
//builder.Services.AddHostedService<ProcessedVideoStreamConsumerTask>();
builder.Services.AddMvc();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("customPolicy", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.MapReverseProxy();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run("http://*:5000");
