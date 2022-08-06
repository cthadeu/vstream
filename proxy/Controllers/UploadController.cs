using Microsoft.AspNetCore.Mvc;
using video_streamming_proxy.Domain;
using video_streamming_proxy.RabbitMq;

namespace video_streamming_proxy.Controllers;

public class VideoUpload
{
    public IFormFile VideoFile { get; set; }
}
public class UploadController : Controller
{
    private readonly IMediaRepository _mediaRepository;

    public UploadController(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new VideoUpload());
    }
    
    [HttpPost]
    [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
    [RequestSizeLimit(209715200)]
    public async Task<IActionResult> Video(VideoUpload videoUpload)
    {
        var videoGuid = Guid.NewGuid();
        var videoIdentifier = videoGuid.ToString().Replace("-","");
        var fileName = Path.GetFileName(videoUpload.VideoFile.FileName);
        var fileExtension = Path.GetExtension(fileName);
        var newFileName = $"{videoIdentifier}{fileExtension}";
        var contentType = videoUpload.VideoFile.ContentType;
        videoUpload.VideoFile.CopyTo(new FileStream($"/app/uploaded/{newFileName}", FileMode.Create));
        ProcessVideoStreamQueue.SendVideoToProcessQueue(newFileName);
        var media = new Media 
        {
            Filename = newFileName,
            Name = newFileName,
            CreatedAt = DateTime.UtcNow,
            Status = MediaStatus.Processing,
            Slug = "test",
            Id = videoGuid.ToString()
        };
        await _mediaRepository.Save(media);
        Console.WriteLine("UPLOADED");
        return RedirectToAction("Index");
    }

    private static string Base64Encode(string plainText) {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}