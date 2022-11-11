namespace video_streamming_proxy.Controllers.Manager.Requests;

public class ProductSectionRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile Thumbnail { get; set; }
}