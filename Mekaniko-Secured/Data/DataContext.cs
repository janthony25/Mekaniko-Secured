using Mekaniko_Secured.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Mekaniko_Secured.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seed the admin user
            modelBuilder.Entity<User>()
                .HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Carlupcupan@ntonio!09"),
                    Role = "Admin"
                });
        }

    }
}
