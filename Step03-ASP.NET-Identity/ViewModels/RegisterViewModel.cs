using System.ComponentModel.DataAnnotations;

namespace Step03_ASP.NET_Identity.ViewModels
{
  public class RegisterViewModel
  {
    [Required]
    [EmailAddress(ErrorMessage = "Felaktig e-post adress")]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
    [Required]
    public string? ConfirmPassword { get; set; }
    public bool isAdmin { get; set; }
  }
}