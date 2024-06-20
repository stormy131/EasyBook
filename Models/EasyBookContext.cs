using Microsoft.EntityFrameworkCore;

namespace EasyBook.Models;

public class EasyBookContext : DbContext{
    public EasyBookContext(DbContextOptions<EasyBookContext> opts)
        : base(opts)
    {}

    public DbSet<BookItem> BookItems { get; set; } = null!;
}