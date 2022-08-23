namespace video_streamming_proxy.Domain;

public enum UserType
{
    Admin,
    Default
}
public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public UserType UserType { get; set; }
    public IEnumerable<Course> Courses { get; set; }
    
}

