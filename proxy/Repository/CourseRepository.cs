﻿using Dapper;
using System.Data;
using video_streamming_proxy.Domain;
using System.Linq;

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
            var query = @"select courses.*, course_prices.amount from courses
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

        public async Task<Course> GetBySlug(string slug)
        {
            var query = @"select c.*, course_prices.amount from courses c                           
                          left join course_prices on (course_prices.course_id = c.id)
                          where course_prices.active = 1 or course_prices.id is null and c.slug = @slug";
            var result = await _connection.QueryAsync<Course>(query, new { slug });
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Course>> GetByUser(string userId)
        {
            var query = @"select c.* from courses c 
                          inner join user_courses uc on (uc.course_id = c.id) 
                          where uc.user_id = @id";
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
    }
}
