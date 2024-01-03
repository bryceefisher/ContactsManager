using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO;

public class LoginDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; } 
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}