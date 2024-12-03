using System.ComponentModel.DataAnnotations;

namespace Cafee_Prototype.ViewModels;

public class AddUserViewModel
{
    [Required]
    public string UserName { get; set;} = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords not matching")]
    public string ConfirmPassword { get; set; } = string.Empty;
}