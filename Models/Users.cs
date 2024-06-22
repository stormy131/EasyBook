using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EasyBook.Models;

public class UserDTO {
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
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

    public static implicit operator UserDTO(User u){
        return new UserDTO{
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName
        };
    }
}