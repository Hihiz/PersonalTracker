using Microsoft.EntityFrameworkCore;
using PersonalTracker.Models;

namespace PersonalTracker.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
