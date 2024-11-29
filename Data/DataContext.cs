using burger_mania_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users {get; set;}
    }
}