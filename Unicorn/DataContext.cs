using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Unicorn.Entities;

namespace Unicorn
{
    public class DataContext : IdentityDbContext<User>
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Configuration.GetConnectionString("UnicornDatabase"));
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Action> Actions { get; set; }
    }
}
