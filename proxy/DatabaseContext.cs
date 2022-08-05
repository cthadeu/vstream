using Microsoft.EntityFrameworkCore;
using video_streamming_proxy.Domain;

public class DatabaseContext: DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
    {
    }

    public DbSet<Media> Medias => Set<Media>();

}