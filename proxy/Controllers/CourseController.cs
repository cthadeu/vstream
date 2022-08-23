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

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetCourseBySlug([FromRoute] string slug)
        {
            var item = await this.courseRepository.GetBySlug(slug);                                   
            ViewBag.Detail = item;
            return View("Detail");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCoursesFromUser([FromRoute] string userId)
        {
            var items = await this.courseRepository.GetByUser(userId);
            ViewBag.Courses = items.ToArray();
            return View("Courses");
        }

        [HttpGet("user/{userId}/course/{courseId}")]
        public async Task<IActionResult> WatchMode([FromRoute] string userId, [FromRoute] string courseId)
        {
            var items = await this.courseRepository.GetByUser(userId);
            ViewBag.Course = items.Where(x => x.Id == courseId).First();
            return View("Watch");
        }
    }
}
