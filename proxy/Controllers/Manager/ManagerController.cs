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

public class ModuleRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public IFormFile Thumbnail { get; set; }
    public IFormFile VideoFile { get; set; }
}

public class PriceRequest
{
    public decimal Price { get; set;}
}


[Route("manager")]
public class ManagerController : Controller
{
    private readonly ICourseRepository courseRepository;
    private readonly IMediaRepository mediaRepository;

    public ManagerController(ICourseRepository courseRepository, IMediaRepository mediaRepository)
    {
        this.courseRepository = courseRepository;
        this.mediaRepository = mediaRepository;
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
    
    [HttpGet("products/{id}/sections")]
    public async Task<IActionResult> ProductsModules(string id)
    {
        var course = await courseRepository.GetById(id);       
        ViewBag.Course = course; 
        return View("ProductsSections");
    }
    
    [HttpGet("products/{id}/sections/add")]
    public async Task<IActionResult> AddProductsModule(string id)
    {
        var course = await courseRepository.GetById(id);       
        ViewBag.Course = course; 
        return View("ProductsSectionsForm");
    }

    [HttpGet("products/add")]
    public IActionResult ProductsAdd(){

        return View("ProductsForm", new ProductRequest());
    }
    
    [HttpPost("products/{id}/sections")]
    [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
    [RequestSizeLimit(209715200)]
    public async Task<IActionResult> SaveModule(string id, ModuleRequest moduleRequest)
    {   
        var tempFile = Path.GetTempFileName();
        using (var stream = System.IO.File.Create(tempFile)) 
        {
            moduleRequest.Thumbnail.CopyTo(stream);
        }
            

        byte[] imageData = await System.IO.File.ReadAllBytesAsync(tempFile);
        var mediaId = Guid.NewGuid().ToString("N");
        var chapter = new Chapter() 
        {
            Title = moduleRequest.Title,
            Description = moduleRequest.Description,
            Id = Guid.NewGuid().ToString(),
            Thumbnail = Convert.ToBase64String(imageData),
            Media = new Media
            {
                Id = mediaId,
                Name = mediaId,
                Slug = moduleRequest.Slug,
                Filename = "",
                Status = MediaStatus.Processing,
                CreatedAt = DateTime.UtcNow
            }
        };
        await mediaRepository.Save(chapter.Media);
        await courseRepository.SaveChapter(chapter, id);
        return Redirect($"/manager/products/{id}/sections");
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

    [HttpGet("products/{id}/prices")]
    public async Task<IActionResult> GetPrice(string id)
    {   
        
        var course = await courseRepository.GetById(id);
        ViewBag.Course = course;
        return View("ProductsPrices");
    }

    [HttpPost("products/{id}/prices")]
    public async Task<IActionResult> SaveNewPrice(string id, PriceRequest priceRequest)
    {   
        await courseRepository.SaveNewPrice(priceRequest.Price, id);
        return Redirect($"/manager/products");
    }

}
