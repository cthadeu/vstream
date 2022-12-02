using System.Data;
using System.Text.Json;
using MongoDB.Driver;
using video_streamming_proxy.Controllers.Manager;
using video_streamming_proxy.Domain;

namespace video_streamming_proxy.Repository;

public class CourseMongoRepository: ICourseRepository
{
    private IMongoCollection<Course> _courseCollection;
    private IMongoCollection<User> _userCollection;

    public CourseMongoRepository(IMongoDatabase mongoDatabase)
    {
        _courseCollection = mongoDatabase.GetCollection<Course>("courses");
        _userCollection = mongoDatabase.GetCollection<User>("users");
    }

    public async Task Save(Course course) => await _courseCollection.InsertOneAsync(course);

    public async Task<IEnumerable<Course>> GetAll()
    {
      var result = await _courseCollection.FindAsync(_ => true);
      return result.ToEnumerable();
    } 
    
    public async Task<IEnumerable<Chapter>> GetChapters(string courseId)
    {
        var result = await _courseCollection.FindAsync(x => x.Id == courseId);
        var course = await result.FirstOrDefaultAsync();
        return course.Chapters;
    }

    public async Task<Course> GetBySlug(string slug)
    {
        var result = await _courseCollection.FindAsync(x => x.Slug == slug);
        var course = await result.FirstOrDefaultAsync();
        return course;
    }

    public async Task<IEnumerable<Course>> GetByUser(string userId)
    {
        var result = await _userCollection.FindAsync(x => x.Id == userId);
        var user = await result.FirstOrDefaultAsync();
        return user.Courses;
    }
   
    public async Task<Course> GetById(string courseId)
    {
        var result = await _courseCollection.FindAsync(x => x.Id == courseId);
        var course = await result.FirstOrDefaultAsync();
        return course;
    }

    public async Task SaveChapter(Chapter chapter, string courseId)
    {
        var result = await _courseCollection.FindAsync(x => x.Id == courseId);
        var course = await result.FirstOrDefaultAsync();
        course.Chapters = course.Chapters == null ? new List<Chapter>() : course.Chapters.Append(chapter);
        await _courseCollection.ReplaceOneAsync(x => x.Id == courseId, course);
    }

    public async Task SaveNewPrice(Price price)
    {
        var result = await _courseCollection.FindAsync(x => x.Id == price.CourseId);
        var course = await result.FirstOrDefaultAsync();
        course.Prices = course.Prices == null ? new List<Price> { price } : course.Prices.Append(price);
        await _courseCollection.ReplaceOneAsync(x => x.Id == price.CourseId, course);
    }

    public async Task Update(Course course)
    {
        await _courseCollection.ReplaceOneAsync(x => x.Id == course.Id, course);
    }
}