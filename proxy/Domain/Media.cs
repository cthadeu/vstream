namespace video_streamming_proxy.Domain; 

public enum MediaStatus {
    Processing,
    Completed
}
public class Media {
    public Guid Id { get; set; }
    public string Filename { get; set; }
    public DateTime CreatedAt { get; set;}
    public string Name { get; set; }
    public string Slug { get; set; }
    public MediaStatus Status { get; set; }
}