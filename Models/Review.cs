using System.ComponentModel.DataAnnotations;
using EasyBook.Filters;

namespace EasyBook.Models;

public class ReviewDTO{
    public long Id { get; set; }

    [Required(AllowEmptyStrings=true)]
    public string? Text { get; set; }

    public long UserId { get; set; }

    [Range(1, 5)]
    public required int Rating { get; set; }   

    public static explicit operator ReviewDTO(ReviewItem r){
        return new ReviewDTO{
            Id = r.Id,
            Text = r.Text,
            UserId = r.UserId,
            Rating = r.Rating
        };
    }

    public static implicit operator ReviewItem(ReviewDTO r){
        return new ReviewItem{
            Rating = r.Rating,
            Text = r.Text,
            UserId = r.UserId,
            Id = r.Id
        };
    }
}

public class ReviewItem : ICreatedByUser{
    public long Id { get; set; }

    [Required(AllowEmptyStrings=true)]
    public string? Text { get; set; }

    public long UserId { get; set; }
    [Required]
    public virtual User User { get; set; }

    public long BookItemId { get; set; }
    public virtual BookItem BookItem { get; set; }

    [Range(1, 5)]
    public required int Rating { get; set; } 
}