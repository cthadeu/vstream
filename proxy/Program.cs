
using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;
using Npgsql;
using video_streamming_proxy.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddScoped<IDbConnection>((sp) => {
    return new NpgsqlConnection("Server=localhost;Port=5432;Database=vstream;User Id=postgres;Password=postgres");
});
builder.Services.AddScoped<IMongoDatabase>(provider =>
{
    var credential = new MongoCredential(
        "SCRAM-SHA-1", 
        new MongoInternalIdentity("vstream", "vstream"),
        new PasswordEvidence("vstream"));
    var mongoServer = new MongoServerAddress("localhost");
    var settints = new MongoClientSettings();
    settints.Credential = credential;
    settints.Server = mongoServer;
    
    var mongoClient = new MongoClient(settints);
    return mongoClient.GetDatabase("vstream");
});
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<ICourseRepository, CourseMongoRepository>();
builder.Services.AddScoped<IUserRepository, UserMongoRepository>();
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

app.Run("http://*:5003");
