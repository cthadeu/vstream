﻿using Dapper;
using System.Data;
using video_streamming_proxy.Domain;

namespace video_streamming_proxy.Repository
{
    public interface ICourseRepository
    {
        Task Save(Course course);
        Task<IEnumerable<Course>> GetAll();

        Task<IEnumerable<Chapter>> GetChapters(string courseId);
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
            var query = "select * from courses order by created_at desc";
            var result = await _connection.QueryAsync<Course>(query);
            return result;
        }


        public async Task<IEnumerable<Chapter>> GetChapters(string courseId)
        {
            var query = @"select * from courses 
                          inner join chapters on chapters.course_id = courses.id
                          inner join media on chapters.media_id = media.id 
                          where courses.id = @id";
            var result = await _connection.QueryAsync<Chapter>(query, new { id = courseId });
            return result;
        }

        public async Task Save(Course course)
        {
            var insert = @"insert into courses(id, name, description, status, slug, created_at) 
                           values(@id, @name, @description, @status, @slug, @created_at)";

            var parameters = new
            {
                id = course.Id,
                name = course.Name,
                description = course.Description,
                status = course.Status,
                slug = course.Slug,
                created_at = course.CreatedAt
            };

            await _connection.ExecuteAsync(insert, parameters);
        }
    }
}