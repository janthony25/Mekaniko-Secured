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
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Make> Makes { get; set; }
        public DbSet<CarMake> CarMakes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }

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
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Calupcupan@ntonio!09"),
                    Role = "Admin"
                });

            // 1-to-M Customer-Car
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Car)
                .WithOne(car => car.Customer)
                .HasForeignKey(car => car.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // M-to-M Car-Make
            modelBuilder.Entity<CarMake>()
                .HasKey(cm => new { cm.CarId, cm.MakeId });

            modelBuilder.Entity<CarMake>()
                .HasOne(cm => cm.Car)
                .WithMany(car => car.CarMake)
                .HasForeignKey(cm => cm.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarMake>()
                .HasOne(cm => cm.Make)
                .WithMany(make => make.CarMake)
                .HasForeignKey(cm => cm.MakeId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-M Car-Invoice
            modelBuilder.Entity<Car>()
                .HasMany(car => car.Invoice)
                .WithOne(i => i.Car)
                .HasForeignKey(i => i.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-M Invoice-InvoieItem
            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.InvoiceItem)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-M Car-Quotation
            modelBuilder.Entity<Car>()
                .HasMany(car => car.Quotation)
                .WithOne(q => q.Car)
                .HasForeignKey(q => q.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-M Quotation-QuotationItem
            modelBuilder.Entity<Quotation>()
                .HasMany(q => q.QuotationItem)
                .WithOne(qi => qi.Quotation)
                .HasForeignKey(qi => qi.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);
                

        }

    }
}
