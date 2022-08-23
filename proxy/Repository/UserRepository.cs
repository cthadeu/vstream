using System.Data;
using video_streamming_proxy.Domain;
using Dapper;

namespace video_streamming_proxy.Repository;

public interface IUserRepository
{
    Task Save(User user);
    Task AddCourse(Course course, User user);
    Task<IEnumerable<User>> GetUsers();

    Task<User> FindById(string id);

    Task<User> FindByEmailAndPassword(string email, string password);
}
public class UserRepository : IUserRepository
{
    private readonly IDbConnection connection;

    public UserRepository(IDbConnection connection)
    {
        this.connection = connection;
    }

    public async Task AddCourse(Course course, User user)
    {
        var insert = @"insert into user_courses(user_id, course_id)
                        values(@userId, @courseId)";
        await this.connection.ExecuteAsync(insert, new { userId = user.Id, courseId = course.Id });
    }

    public async Task<User> FindByEmailAndPassword(string email, string password)
    {
        var query = "select * from users where email = @email and password = @password";
        var result = await this.connection.QueryAsync<User>(query, new { email, password });
        return result.FirstOrDefault();
    }

    public async Task<User> FindById(string id)
    {
        var sql = @"select * from users where id = @id;
                    select c.* from user_courses uc
                    inner join courses c on (uc.course_id = c.id)
                    where uc.user_id = @id";

        var result = await connection.QueryMultipleAsync(sql, new { id });
        var userResult = await result.ReadAsync<User>();
        var coursesResult = await result.ReadAsync<Course>();

        var user = userResult.First();
        user.Courses = coursesResult.ToList();
        return user;
    }

    public Task<IEnumerable<User>> GetUsers()
    {
        throw new NotImplementedException();
    }

    public async Task Save(User user)
    {
        var insert = @"insert into users(id, name, email, phone, password, user_type) 
                        values(@id, @name, @email, @phone, @password, @userType)";

        var parameters = new
        {
            id = user.Id,
            name = user.Name,
            email = user.Email,
            phone = user.Phone,
            password = user.Password,
            userType = user.UserType
        };
        await connection.ExecuteAsync(insert, parameters);
    }
}

