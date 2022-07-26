using System.Data;
using video_streamming_proxy.Domain;
using Dapper;

public interface IMediaRepository
{
    Task Save(Media media);
    Task Update(Media media);
    Task<IEnumerable<Media>> GetAll();
    Task<Media> GetByFilename(string filename);

    Task<Media> GetById(Guid id);
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

    public async Task<Media> GetByFilename(string filename)
    {
        var query = "select * from media where name = @filename";
        var result = await _connection.QueryAsync<Media>(query, new { filename=filename});
        return result.FirstOrDefault();
    }

    public async Task<Media> GetById(Guid id)
    {
         var query = "select * from media where id = @id";
        var result = await _connection.QueryAsync<Media>(query, new { id=$"{id.ToString()}" });
        return result.FirstOrDefault();
    }
    

    public async Task Save(Media media)
    {
        var insert = @"insert into media(id, filename, name, created_at, slug, status) 
                        values (@id, @filename, @name, @createdAt, @slug, @status)";
        var parameters = new 
        {
            id = media.Id,
            filename = media.Filename,
            name = media.Name,
            createdAt = media.CreatedAt,
            status = media.Status,
            slug = media.Slug
        };
        await _connection.ExecuteAsync(insert, parameters);
    }

    public async Task Update(Media media)
    {
        var update = @"update media set 
                        filename = @filename, 
                        name = @name,
                        status = @status
                        where id = @id";
        var parameters = new 
        {
            id = media.Id,
            filename = media.Filename,
            name = media.Name,
            status = media.Status,
        };

        await _connection.ExecuteAsync(update, parameters);
    }
}