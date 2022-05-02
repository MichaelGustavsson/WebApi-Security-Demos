using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Step01_TheBasics.ViewModels;

namespace Step01_TheBasics.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    [HttpPost]
    public IActionResult Authenticate(LoginViewModel model)
    {
      if (model.UserName == "kalle" && model.Password == "Pa$$w0rd")
      {
        var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, "kalle"),
                    new Claim(ClaimTypes.Email, "kalle@gmail.com"),
                    new Claim("User", "true")
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
      var key = Encoding.ASCII.GetBytes("daldfdlwij4ovdsnvnböajrqewojoffklsgöajkgölj");

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