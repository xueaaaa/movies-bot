using Microsoft.EntityFrameworkCore;

namespace MoviesBot
{
    public class BotContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }

        public BotContext() : base() 
        {
            Database.EnsureCreated();
        }

        public int AddEntity<T>(T enitity)
        {
            Add(enitity);
            return SaveChanges();
        }

        public async Task<int> AddEntityAsync<T>(T enitity)
        {
            await AddAsync(enitity);
            return await SaveChangesAsync();
        }

        public int UpdateEnitity<T>(T enitity)
        {
            Update(enitity);
            return SaveChanges();
        }

        public async Task<int> UpdateEnitityAsync<T>(T enitity)
        {
            Update(enitity);
            return await SaveChangesAsync();
        }

        public int RemoveEntity<T>(T enitity)
        {
            Remove(enitity);
            return SaveChanges();
        }

        public async Task<int> RemoveEntityAsync<T>(T enitity)
        {
            Remove(enitity);
            return await SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=movies.db");
        }
    }
}
