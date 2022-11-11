using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
            var item = await this.courseRepository.GetBySlug(slug);                                   
            ViewBag.Detail = item;
            return View("Detail");
        }

        [Authorize]
        [HttpGet("user/")]
        public async Task<IActionResult> GetCoursesFromUser([FromRoute] string userId)
        {
            var items = await this.courseRepository.GetByUser(userId);
            ViewBag.Courses = items.ToArray();
            return View("Courses");
        }


    }
}
