using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{
    public class MyContext: DbContext
    {
        public MyContext(DbContextOptions options) : base(options) {}

        public DbSet<User> users {get;set;}
        public DbSet<Wedding> weddings {get;set;}
        public DbSet<Association> associations {get;set;}
    }
}