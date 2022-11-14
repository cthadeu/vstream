using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using video_streamming_proxy.Repository;

namespace video_streamming_proxy.Controllers;

public class AuthController : Controller
{
    private readonly IMediaRepository _mediaRepository;
    private readonly ICourseRepository courseRepository;
    private readonly IUserRepository userRepository;

    public AuthController(IMediaRepository mediaRepository, 
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _mediaRepository = mediaRepository;
        this.courseRepository = courseRepository;
        this.userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult Index() 
    {
        return View("home/index");
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await userRepository.FindByEmailAndPassword(username, password);
        Console.WriteLine(user);
        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim("user", user.Name),
                new Claim("id", user.Id),
                new Claim("email", user.Email),
                new Claim("role", user.UserType.ToString())
            };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));
            return Redirect("/home/content");
            
        }

        return Redirect("/auth");
    }


}