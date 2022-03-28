using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StepApi.Identity.Model;

namespace StepApi.Identity.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions options) : base(options){}


        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
