using Microsoft.EntityFrameworkCore;
using UserCRUD.Models;

namespace UserCRUD.Data
{
    public class ApiTokenDbContext : DbContext
    {
        public ApiTokenDbContext(DbContextOptions<ApiTokenDbContext> options) : base(options)
        {
        }
        public DbSet<UserToken> Tokens { get; set; }
    }
}
