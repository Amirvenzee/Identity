using IdentityPractice.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityPractice.Context
{
    public class TestDbContext:IdentityDbContext<ApplicationUser>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options):base(options)
        {
            
        }
        public DbSet<Employee> Employess { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Employee>().HasIndex(x => x.Name).IsUnique();
      

            base.OnModelCreating(builder);
        }

    }
}
