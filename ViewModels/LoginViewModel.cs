using System.ComponentModel.DataAnnotations;

namespace Cafee_Prototype.ViewModels;

public class LoginViewModel
{
    public string UserName { get; set; } = String.Empty;
    
    [DataType(DataType.Password)]
    public string Password { get; set; } = String.Empty;

}
