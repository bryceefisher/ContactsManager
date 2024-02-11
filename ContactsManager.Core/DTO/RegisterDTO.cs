using System.ComponentModel.DataAnnotations;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;


namespace ContactsManager.Core.DTO;

public class RegisterDto
{
    [Remote(action: "UniqueEmail", controller: "Account", ErrorMessage = "Email is already in use")]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Person name is required")]
    public string PersonName { get; set; }
    
    [Required(ErrorMessage = "Phone is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    
    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
    public string ConfirmPassword { get; set; } = null!;

    public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
}