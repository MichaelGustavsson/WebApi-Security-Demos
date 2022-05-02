using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Step04_Authorization.Data
{
  public class ApplicationContext : IdentityDbContext
  {
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }
  }
}