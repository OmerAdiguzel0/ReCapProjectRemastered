using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework
{
    public class RentACarContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=rentacar;Username=oemiar;Password=1");
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<CarImage> CarImages { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tablo isimlerini küçük harfle yazalım (PostgreSQL convention)
            modelBuilder.Entity<Car>().ToTable("cars");
            modelBuilder.Entity<Brand>().ToTable("brands");
            modelBuilder.Entity<Color>().ToTable("colors");
            modelBuilder.Entity<Customer>().ToTable("customers");
            modelBuilder.Entity<Rental>().ToTable("rentals");
            modelBuilder.Entity<CarImage>().ToTable("car_images");
            modelBuilder.Entity<OperationClaim>().ToTable("operation_claims");
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<UserOperationClaim>().ToTable("user_operation_claims");

            // Primary key tanımlamaları
            modelBuilder.Entity<Car>().HasKey(c => c.CarId);
            modelBuilder.Entity<Brand>().HasKey(b => b.BrandId);
            modelBuilder.Entity<Color>().HasKey(c => c.ColorId);
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
            modelBuilder.Entity<Rental>().HasKey(r => r.RentalId);
            modelBuilder.Entity<CarImage>().HasKey(ci => ci.CarImageId);
            modelBuilder.Entity<OperationClaim>().HasKey(oc => oc.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<UserOperationClaim>().HasKey(uoc => uoc.Id);

            // İlişkiler
            modelBuilder.Entity<Car>()
                .HasOne<Brand>()
                .WithMany()
                .HasForeignKey(c => c.BrandId);

            modelBuilder.Entity<Car>()
                .HasOne<Color>()
                .WithMany()
                .HasForeignKey(c => c.ColorId);

            modelBuilder.Entity<Customer>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Rental>()
                .HasOne<Car>()
                .WithMany()
                .HasForeignKey(r => r.CarId);

            modelBuilder.Entity<Rental>()
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(r => r.CustomerId);

            modelBuilder.Entity<CarImage>()
                .HasOne<Car>()
                .WithMany()
                .HasForeignKey(ci => ci.CarId);

            modelBuilder.Entity<UserOperationClaim>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(uoc => uoc.UserId);

            modelBuilder.Entity<UserOperationClaim>()
                .HasOne<OperationClaim>()
                .WithMany()
                .HasForeignKey(uoc => uoc.OperationClaimId);
        }
    }
}
