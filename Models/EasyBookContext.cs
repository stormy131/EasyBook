using Microsoft.EntityFrameworkCore;

namespace EasyBook.Models;

public class EasyBookContext : DbContext{
    public EasyBookContext(DbContextOptions<EasyBookContext> opts)
        : base(opts)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cascade deletion
        modelBuilder.Entity<Order>()
                    .HasMany(x => x.OrderedItems)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);

        // Pre-filling database for testing
        modelBuilder.Entity<User>().HasData( new User{
            Id = 1,
            LastName = "A",
            FirstName = "A",
            Password = "123",
            Email = "123@example.com",
            IsAdmin=true
        } );
        modelBuilder.Entity<User>().HasData( new User{
            Id = 2,
            FirstName = "B",
            LastName = "B",
            Password = "qwe",
            Email = "qwe@example.com",
            IsAdmin=false
        });

        modelBuilder.Entity<BookItem>().HasData(new BookItem{
            Id = 1,
            Name = "1",
            Author = "1",
            Price = 10,
            Genre = "Horror"
        });
        modelBuilder.Entity<BookItem>().HasData(new BookItem{
            Id = 2,
            Name = "2",
            Author = "2",
            Price = 3,
            Genre = "Detective"
        });

        modelBuilder.Entity<ReviewItem>().HasData( new ReviewItem{
            Id = 1,
            Text = "Awesome book",
            Rating = 3,
            UserId = 1,
            BookItemId = 1,

        });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<BookItem> BookItems { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<ReviewItem> Reviews { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
}