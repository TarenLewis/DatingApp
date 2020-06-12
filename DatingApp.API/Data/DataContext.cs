using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
    
    // Name "Values" used to represent the table name that gets created when the database is "Scaffolded"
        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }

    }
}