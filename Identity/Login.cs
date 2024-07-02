using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyBook.Identity;

public class LoginData{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; }
}