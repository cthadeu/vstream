using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using video_streamming_proxy.Repository;


namespace video_streamming_proxy.Controllers
{
    [Route("user")]
    public class UserController: Controller
    {
        private readonly ICourseRepository courseRepository;
        private readonly IUserRepository userRepository;

        public UserController(ICourseRepository courseRepository, IUserRepository userRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
        }


        [Authorize]
        [HttpGet("courses")]
        public async Task<IActionResult> GetCoursesFromUser()
        {
            var userId = HttpContext.User.Claims.Where(x => x.Type == "id").First().Value;
            var items = await this.courseRepository.GetByUser(userId);
            ViewBag.Courses = items.ToArray();
            return View("Courses");
        }

        [Authorize]
        [HttpGet("courses/{courseId}")]
        public async Task<IActionResult> WatchMode([FromRoute] string courseId, [FromQuery] string chapter)
        {
            var userId = HttpContext.User.Claims.Where(x => x.Type == "id").First().Value;
            
            var items = await this.courseRepository.GetByUser(userId);
            var course = items.Where(x => x.Id == courseId).First();
            var chapters = await this.courseRepository.GetChapters(courseId);
            var selectedChapter = string.IsNullOrEmpty(chapter) ? chapters.First() : chapters.First(x => x.Id == chapter);            
            ViewBag.Course = course;
            ViewBag.Chapters = chapters.ToArray();
            ViewBag.SelectedChapter = selectedChapter;
            return View("Watch");
        }

        [HttpGet("courses/{slug}/purchase")]
        public async Task<IActionResult> ConfirmPurchase([FromRoute] string slug)
        {
            var userId = HttpContext.User.Claims.Where(x => x.Type == "id").First().Value;
            var item = await this.courseRepository.GetBySlug(slug);
            var userCourses = await this.courseRepository.GetByUser(userId);
            ViewBag.UserHasCourse = userCourses.Any(x => x.Id == item.Id);
            ViewBag.Detail = item;
            return View();
        }

        [Authorize]
        [HttpPost("courses/{slug}/purchase")]
        public async Task<IActionResult> Purchase([FromRoute] string slug)
        {
            var userId = HttpContext.User.Claims.Where(x => x.Type == "id").First().Value;
            var item = await this.courseRepository.GetBySlug(slug);
            ViewBag.Detail = item;
            try {
                await this.userRepository.AddCourse(item, new Domain.User(userId));
                return View("PurchaseConfirmation");
            } catch(Exception e) 
            {
                return this.BadRequest("Não foi possivel concluir a operação");
            }
            
            
        }

    }
}
