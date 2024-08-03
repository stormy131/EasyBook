using System.ComponentModel.DataAnnotations;

namespace EasyBook.Models;

public class BookItemDTO{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string Author { get; set; }

    public required int Price { get; set; }
    public string? Genre { get; set; }

    public static implicit operator BookItem(BookItemDTO b){
        return new BookItem{
            Id = b.Id,
            Name = b.Name,
            Author = b.Author,
            Genre = b.Genre,
            Price = b.Price,
        };
    }

    public static explicit operator BookItemDTO(BookItem b){
        return new BookItemDTO{
            Id = b.Id,
            Name = b.Name,
            Author = b.Author,
            Genre = b.Genre,
            Price = b.Price,
        };
    }
}

public class BookItem {
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string Author { get; set; }

    public required int Price { get; set; }
    public string? Genre { get; set; }
    
    public double Rating {
        get {
            if(Reviews is null || !Reviews.Any()){
                return 0;
            }

            return Reviews.Select(r => r.Rating).Average();
        }
    }

    public virtual IEnumerable<ReviewItem> Reviews { get; set; }
}