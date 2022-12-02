using Dapper;
using System.Data;
using video_streamming_proxy.Domain;
using System.Linq;
using System.Text.Json;

namespace video_streamming_proxy.Repository
{
    public interface ICourseRepository
    {
        Task Save(Course course);
        Task<IEnumerable<Course>> GetAll();
        Task<IEnumerable<Chapter>> GetChapters(string courseId);
        Task<Course> GetBySlug(string slug);
        Task<IEnumerable<Course>> GetByUser(string userId);
        Task<Course> GetById(string courseId);
        Task SaveChapter(Chapter chapter, string courseId);

        Task SaveNewPrice(Price price);

        Task Update(Course course);
    }
    public class CourseRepository: ICourseRepository
    {
        private readonly IDbConnection _connection;

        public CourseRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Course>> GetAll()
        {
            var query = @"select courses.*, course_prices.plan, course_prices.amount from courses
                            left join course_prices on course_prices.course_id = courses.id
                            where course_prices.amount is null or course_prices.active = 1
                            order by courses.created_at desc, course_prices.created_at desc";
            
            var result = await _connection.QueryAsync<Course>(query);
            return result;
        }

        public async Task<Course> GetById(string courseId)
        {
            var queries = @"select * from courses where id = @courseId;
                            select * from modules where course_id = @courseId";

            var result = await _connection.QueryMultipleAsync(queries, new { courseId });
            var course = await result.ReadFirstAsync<Course>();
            var chapters = await result.ReadAsync<Chapter>();
            if (course != null) 
                course.Chapters = chapters;
            
            return course;
        }

        public async Task SaveChapter(Chapter chapter, string courseId)
        {
            var insert = @"insert into modules(id, thumbnail, title, description, course_id, media_id) 
                           values(@id, @thumbnail, @title, @description, @course_id, @media_id)";

            var parameters = new
            {
                id = chapter.Id,
                title = chapter.Title,
                description = chapter.Description,
                thumbnail = chapter.Thumbnail,
                course_id = courseId,
                media_id = chapter.Media.Id
            };

            await _connection.ExecuteAsync(insert, parameters);
        }

        public async Task<Course> GetBySlug(string slug)
        {
            var query = @"select c.*, course_prices.amount from courses c                           
                          left join course_prices on (course_prices.course_id = c.id)
                          where c.slug = @slug and (course_prices.active = 1 or course_prices.id is null)";
            var result = await _connection.QueryAsync<Course>(query, new { slug });
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Course>> GetByUser(string userId)
        {
            
            var query = @"select courses.*, course_prices.amount from courses
                            inner join user_courses uc on (uc.course_id = courses.id)
                            left join course_prices on course_prices.course_id = courses.id
                            where uc.user_id = @id and (course_prices.amount is null or course_prices.active = 1)
                            order by courses.created_at desc, course_prices.created_at desc";
            var result = await _connection.QueryAsync<Course>(query, new { id = userId });
            return result;
        }

        public async Task<IEnumerable<Chapter>> GetChapters(string courseId)
        {
            var query = @"select * from modules                           
                          inner join media on modules.media_id = media.id 
                          where modules.course_id = @id";
            var result = await _connection.QueryAsync<Chapter, Media, Chapter>(query, (chapter, media) => 
            {
                chapter.Media = media;
                return chapter;
            } ,new { id = courseId });            
            return result;
        }

        

        public async Task Save(Course course)
        {
            var insert = @"insert into courses(id, name, description, status, slug, created_at, thumbnail) 
                           values(@id, @name, @description, @status, @slug, @created_at, @thumbnail)";

            var parameters = new
            {
                id = course.Id,
                name = course.Name,
                description = course.Description,
                status = course.Status,
                slug = course.Slug,
                created_at = course.CreatedAt,
                thumbnail = course.Thumbnail
            };

            await _connection.ExecuteAsync(insert, parameters);
        }

        public async Task SaveNewPrice(Price price)
        {
            var update = @"update course_prices set active = 0 where course_id = @course_id and plan = @plan";

            var insert = @"insert into course_prices(id, course_id, amount, created_at, active, plan) 
                           values(@id, @courseId, @price, @created_at, @active, @plan)";

            var parameters = new
            {
                id = price.Id,
                courseId = price.CourseId,
                price = price.Amount,
                created_at = price.CreatedAt,
                active = price.Active,
                plan = price.Plan.ToString() 
            };
            
            Console.WriteLine(JsonSerializer.Serialize(parameters));

            await _connection.ExecuteAsync(update, new { course_id = price.CourseId, plan = price.Plan.ToString() });
            await _connection.ExecuteAsync(insert, parameters);
            
        }

        public Task Update(Course course)
        {
            throw new NotImplementedException();
        }
    }
}
