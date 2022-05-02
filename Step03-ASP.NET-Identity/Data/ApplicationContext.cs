using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Step03_ASP.NET_Identity.Data
{
  public class ApplicationContext : IdentityDbContext
  {
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }
  }
}