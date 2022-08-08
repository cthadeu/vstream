using Microsoft.AspNetCore.Mvc;
using video_streamming_proxy.Repository;

namespace video_streamming_proxy.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseRepository courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }

        
    }
}
