using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Step03_ASP.NET_Identity.ViewModels
{
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase
  {
    private readonly IConfiguration _config;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    public AuthController(IConfiguration config, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {

      var user = await _userManager.FindByNameAsync(model.UserName);

      if (user is null) return Unauthorized("Felaktigt användarnamn");

      var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

      if (!result.Succeeded) return Unauthorized();

      var claims = await _userManager.GetClaimsAsync(user);

      var userData = new UserViewModel
      {
        UserName = user.UserName,
        Token = CreateJwtToken(claims, DateTime.Now.AddMinutes(10))
      };

      return Ok(userData);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      if (model.Password != model.ConfirmPassword) return BadRequest($"Password and ConfirmPassword does not match.");

      var user = new IdentityUser
      {
        Email = model.Email!.ToLower(),
        UserName = model.Email.ToLower()
      };

      var claimRole = model.isAdmin ? new Claim("Admin", "true") : new Claim("User", "true");

      var claimUser = new Claim("UserName", user.UserName);

      var result = await _userManager.CreateAsync(user, model.Password);

      if (result.Succeeded)
      {
        await _userManager.AddClaimAsync(user, claimRole);
        await _userManager.AddClaimAsync(user, claimUser);

        if (result.Succeeded)
        {
          var claims = await _userManager.GetClaimsAsync(user);
          var userData = new UserViewModel
          {
            UserName = user.UserName,
            Token = CreateJwtToken(claims, DateTime.Now.AddMinutes(10)),
          };

          return StatusCode(201, userData);
        }
        else
        {
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError("User registration", error.Description);
          }
          return StatusCode(500, ModelState);
        }
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("User registration", error.Description);
        }
        return StatusCode(500, ModelState);
      }

    }

    private string CreateJwtToken(IList<Claim> claims, DateTime expiresAt)
    {
      // 1. Vi behöver en hemlighet(secret key)
      var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("apiKey"));

      var jwt = new JwtSecurityToken(
          claims: claims,
          notBefore: DateTime.Now,
          expires: expiresAt,
          signingCredentials: new SigningCredentials(
              new SymmetricSecurityKey(key), // 2. Använd hemligheten här för att skapa en hash.
              SecurityAlgorithms.HmacSha512Signature));

      // 3. Slutligen använd JwtSecurityTokenHandler för att att skriva ut token som en sträng...
      return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
  }
}