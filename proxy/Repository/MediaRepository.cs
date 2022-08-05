using System.Data;
using video_streamming_proxy.Domain;
using Dapper;

public interface IMediaRepository
{
    Task Save(Media media);
    Task<IEnumerable<Media>> GetAll();
}

public class MediaRepository : IMediaRepository
{
    private readonly IDbConnection _connection;

    public MediaRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Media>> GetAll()
    {
        var query = "select * from media order by created_at";
        var result = await _connection.QueryAsync<Media>(query);
        return result;
    }

    public async Task Save(Media media)
    {
        var insert = @"insert into media(id, filename, name, created_at, status) 
                        values (@id, @filename, @name, @createdAt, @status)";
        var parameters = new 
        {
            id = media.Id,
            filename = media.Filename,
            name = media.Name,
            createdAt = media.CreatedAt,
            status = media.Status
        };
        await _connection.ExecuteAsync(insert, parameters);
    }


    
}