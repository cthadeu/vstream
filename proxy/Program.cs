
using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddScoped<IDbConnection>((sp) => new NpgsqlConnection("Server=database;Port=5432;Database=vstream;User Id=postgres;Password=postgres"));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.AccessDeniedPath = "/denied";
        options.LoginPath = "/Home";
        options.Cookie.Name = "vstream";

    });
    

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
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.Run("http://*:5000");
