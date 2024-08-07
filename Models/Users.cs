using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace EasyBook.Models;

public class UserDTO {
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsAdmin { get; set; }

    public static implicit operator UserDTO(User u){
        return new UserDTO{
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            IsAdmin = u.IsAdmin
        };
    }
}

public class User{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    [Required]
    [PasswordPropertyText]
    public required string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public bool IsAdmin { get; set; }
}