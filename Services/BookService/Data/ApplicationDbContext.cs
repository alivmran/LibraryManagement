using Microsoft.EntityFrameworkCore;
using Library.BookService.Models;

namespace Library.BookService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Book> Books { get; set; }
    }
}
