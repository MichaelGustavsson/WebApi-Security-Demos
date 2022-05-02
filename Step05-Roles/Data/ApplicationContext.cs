using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Step05_Roles.Data
{
  public class ApplicationContext : IdentityDbContext
  {
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }
  }
}