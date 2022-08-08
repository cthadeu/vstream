namespace video_streamming_proxy.Domain;

public enum CourseStatus
{
    Active,
    Inactive,

}
public class Chapter
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public Media Media { get; set; }
}
public class Course
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public CourseStatus Status { get; set; }
    public string Slug { get; set; }
}