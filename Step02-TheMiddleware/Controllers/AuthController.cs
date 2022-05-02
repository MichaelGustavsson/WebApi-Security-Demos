using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Step02_TheMiddleware.ViewModels;

namespace Step02_TheMiddleware.Controllers
{
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase
  {
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config)
    {
      _config = config;
    }

    [HttpPost]
    public IActionResult Authenticate(LoginViewModel model)
    {
      if (model.UserName == "kalle" && model.Password == "Pa$$w0rd")
      {
        var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, "kalle"),
                    new Claim(ClaimTypes.Email, "kalle@gmail.com"),
                    new Claim("User", "true"),
                    new Claim("Admin", "true")
                };

        var expiresAt = DateTime.Now.AddMinutes(10);

        return Ok(new
        {
          access_token = CreateJwtToken(claims, expiresAt),
          expires_at = expiresAt
        });
      }

      ModelState.AddModelError("Unauthorized", "Felaktig inloggning");

      return Unauthorized(ModelState);
    }

    private string CreateJwtToken(List<Claim> claims, DateTime expiresAt)
    {
      // 1. Vi behöver en hemlighet(secret key)
      var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("apiKey"));

      var jwt = new JwtSecurityToken(
          claims: claims,
          notBefore: DateTime.Now,
          expires: expiresAt,
          signingCredentials: new SigningCredentials(
              new SymmetricSecurityKey(key),
              SecurityAlgorithms.HmacSha512Signature));

      return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
  }
}