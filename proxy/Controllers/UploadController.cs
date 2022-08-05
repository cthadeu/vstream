using Microsoft.AspNetCore.Mvc;
using video_streamming_proxy.RabbitMq;

namespace video_streamming_proxy.Controllers;

public class VideoUpload
{
    public IFormFile VideoFile { get; set; }
}
public class UploadController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new VideoUpload());
    }
    
    [HttpPost]
    [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
    [RequestSizeLimit(209715200)]
    public IActionResult Video(VideoUpload videoUpload)
    {
        
        var fileName = Path.GetFileName(videoUpload.VideoFile.FileName);
        var contentType = videoUpload.VideoFile.ContentType;
        videoUpload.VideoFile.CopyTo(new FileStream($"/app/uploaded/{videoUpload.VideoFile.FileName}", FileMode.Create));
        ProcessVideoStreamQueue.SendVideoToProcessQueue(fileName);
        Console.WriteLine("UPLOADED");
        return RedirectToAction("Index");
    }

    private static string Base64Encode(string plainText) {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}