using MongoDB.Driver;
using video_streamming_proxy.Domain;

namespace video_streamming_proxy.Repository;

public class UserMongoRepository: IUserRepository
{
    private IMongoCollection<User> _userCollection;

    public UserMongoRepository(IMongoDatabase mongoDatabase)
    {
        _userCollection = mongoDatabase.GetCollection<User>("users");
    }

    public async Task Save(User user) => await _userCollection.InsertOneAsync(user); 
    

    public async Task AddCourse(Course course, User user)
    {
        user.Courses = user.Courses.Append(course);
        await _userCollection.ReplaceOneAsync(x => x.Id == user.Id, user);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        var result = await _userCollection.FindAsync(_ => true);
        return result.ToEnumerable();
    }
    

    public async Task<User> FindById(string id)
    {
        var result = await _userCollection.FindAsync(x => x.Id == id);
        return await result.FirstOrDefaultAsync();
    }

    public async Task<User> FindByEmailAndPassword(string email, string password)
    {
        var result = await _userCollection.FindAsync(x => x.Email == email && x.Password == password);
        return await result.FirstOrDefaultAsync();
    }
}