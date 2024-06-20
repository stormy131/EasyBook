namespace EasyBook.Models;

public class BookItem {
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string Author { get; set; }
    public string? Genre { get; set; } 
}