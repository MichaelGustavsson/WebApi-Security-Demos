using System.ComponentModel.DataAnnotations;

namespace Step02_TheMiddleware.ViewModels
{
  public class LoginViewModel
  {
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Password { get; set; }
  }
}