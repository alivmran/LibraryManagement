using Microsoft.EntityFrameworkCore;
using Library.AuthorService.Models;

namespace Library.AuthorService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Author> Authors { get; set; }
    }
}
