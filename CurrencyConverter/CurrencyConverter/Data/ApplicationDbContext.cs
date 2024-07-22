using CurrencyConverter.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConverter.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser>ApplicationUsers { get; set; }
        public DbSet<CurrencyConvert>CurrencyConverts { get; set; }
        public DbSet<FavoriteCurrencyPair> FavoriteCurrencyPairs { get; set; }

    }
}
