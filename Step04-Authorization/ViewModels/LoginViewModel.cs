using System.ComponentModel.DataAnnotations;

namespace Step04_Authorization.ViewModels
{
  public class LoginViewModel
  {
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Password { get; set; }
  }
}