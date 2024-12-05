using Microsoft.EntityFrameworkCore;
using book_dotnet.Models;

namespace book_dotnet.Data
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<LendReturn> LendReturns { get; set; }
    }
}