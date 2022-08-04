
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.AccessDeniedPath = "/denied";
        options.LoginPath = "/Home";
        options.Cookie.Name = "vstream";

    });
    // .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
    // {
    //     c.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
    //     c.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    //     {
    //         ValidAudience = builder.Configuration["Auth0:Audience"],
    //         ValidIssuer = $"{builder.Configuration["Auth0:Domain"]}"
    //     };
    //     
    // });

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
