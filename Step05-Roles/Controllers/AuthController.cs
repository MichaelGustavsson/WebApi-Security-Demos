using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Step05_Roles.ViewModels
{
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase
  {
    private readonly IConfiguration _config;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public AuthController(IConfiguration config, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
      _roleManager = roleManager;
      _signInManager = signInManager;
      _userManager = userManager;
      _config = config;
    }

    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRole([FromQuery] string roleName)
    {

      if (!await _roleManager.RoleExistsAsync(roleName))
      {
        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

        if (!result.Succeeded)
        {
          return BadRequest(result.Errors);
        }
      }
      return StatusCode(201);
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
        Token = await CreateJwtToken(user)
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

      var result = await _userManager.CreateAsync(user, model.Password);

      if (result.Succeeded)
      {
        if (model.isAdmin)
        {
          await _userManager.AddToRoleAsync(user, "Admin");
        }

        await _userManager.AddToRoleAsync(user, "User");

        if (result.Succeeded)
        {
          var userData = new UserViewModel
          {
            UserName = user.UserName,
            Token = await CreateJwtToken(user),
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

    private async Task<string> CreateJwtToken(IdentityUser user)
    {
      // 1. Vi behöver en hemlighet(secret key)
      var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("apiKey"));

      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
      };

      var roles = await _userManager.GetRolesAsync(user);

      claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

      var jwt = new JwtSecurityToken(
          claims: claims,
          notBefore: DateTime.Now,
          expires: DateTime.Now.AddDays(1),
          signingCredentials: new SigningCredentials(
              new SymmetricSecurityKey(key), // 2. Använd hemligheten här för att skapa en hash.
              SecurityAlgorithms.HmacSha512Signature));

      // 3. Slutligen använd JwtSecurityTokenHandler för att att skriva ut token som en sträng...
      return new JwtSecurityTokenHandler().WriteToken(jwt);
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