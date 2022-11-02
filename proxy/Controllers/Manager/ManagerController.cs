using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Data;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using video_streamming_proxy.Repository;
using video_streamming_proxy.Domain;

namespace video_streamming_proxy.Controllers.Manager;

public class ProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public IFormFile Thumbnail { get; set; }

}

[Route("manager")]
public class ManagerController : Controller
{
    private readonly ICourseRepository courseRepository;

    public ManagerController(ICourseRepository courseRepository) 
    {
        this.courseRepository = courseRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("products")]
    public async Task<IActionResult> Products()
    {
        var courses = await this.courseRepository.GetAll();       
        ViewBag.Courses = courses.ToArray(); 
        return View();
    }

    [HttpGet("products/add")]
    public IActionResult ProductsAdd(){

        return View("ProductsForm", new ProductRequest());
    }

    [HttpPost("products/add")]
    public async Task<IActionResult> ProductsSave(ProductRequest productRequest)
    {   
        var tempFile = Path.GetTempFileName();
        using (var stream = System.IO.File.Create(tempFile)) 
        {
            productRequest.Thumbnail.CopyTo(stream);
        }
            

        byte[] imageData = await System.IO.File.ReadAllBytesAsync(tempFile);
        var course = new Course 
        {
            Name = productRequest.Name,
            Description = productRequest.Description,
            Id = Guid.NewGuid().ToString(),
            Status = CourseStatus.Inactive,
            CreatedAt = DateTime.UtcNow,
            Slug = productRequest.Slug,
            Thumbnail = Convert.ToBase64String(imageData)
        };
        await courseRepository.Save(course);
        return RedirectToAction("Products");
    }
}
