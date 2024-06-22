using System.ComponentModel.DataAnnotations;

namespace EasyBook.Models;

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
}