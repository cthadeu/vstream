using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace video_streamming_proxy.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index() 
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (username == "test" && password == "test")
        {
            var claims = new List<Claim>
            {
                new Claim("user", "Test"),
                new Claim("role", "Member")
            };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));
            return Redirect("/Home/Content");
            
        }

        return Redirect("/Home");
    }

    [HttpGet]
    public IActionResult Content()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Videos([FromQuery(Name = "vname")] string vname)
    {
        ViewData["vname"] = vname + ".m3u8";
        return View();
    }
}