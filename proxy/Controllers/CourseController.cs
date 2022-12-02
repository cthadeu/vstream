using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using video_streamming_proxy.Repository;


namespace video_streamming_proxy.Controllers
{
    [Route("courses")]
    public class CourseController : Controller
    {
        private readonly ICourseRepository courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableCourses()
        {
            var courses = await courseRepository.GetAll();       
            ViewBag.Courses = courses.ToArray(); 
            return View("Index");
        }
        

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetCourseBySlug([FromRoute] string slug)
        {
            Console.WriteLine($"Course => {slug}");
            var course = await courseRepository.GetBySlug(slug);      
            Console.WriteLine($"Course => {course.Name}");
            var chapters = await courseRepository.GetChapters(course.Id);
            ViewBag.Course = course;
            ViewBag.Chapters = chapters.ToArray();
            return View("Detail");
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCoursesFromUser([FromRoute] string userId)
        {
            var items = await this.courseRepository.GetByUser(userId);
            ViewBag.Courses = items.ToArray();
            return View("Courses");
        }


    }
}
