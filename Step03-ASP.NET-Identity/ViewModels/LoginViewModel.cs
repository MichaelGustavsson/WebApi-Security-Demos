using System.ComponentModel.DataAnnotations;

namespace Step03_ASP.NET_Identity.ViewModels
{
  public class LoginViewModel
  {
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Password { get; set; }
  }
}