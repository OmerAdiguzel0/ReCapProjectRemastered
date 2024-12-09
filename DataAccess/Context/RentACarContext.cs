using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DataAccess.Models;

namespace DataAccess.Context
{
    public partial class RentACarContext : DbContext
    {
        public RentACarContext()
        {
        }

        public RentACarContext(DbContextOptions<RentACarContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Brand> Brands { get; set; } = null!;
        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<CarImage> CarImages { get; set; } = null!;
        public virtual DbSet<Color> Colors { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<OperationClaim> OperationClaims { get; set; } = null!;
        public virtual DbSet<Rental> Rentals { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserOperationClaim> UserOperationClaims { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=rentacar;Username=oemiar;Password=1");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("brands");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("cars");

                entity.HasIndex(e => e.BrandId, "IX_cars_BrandId");

                entity.HasIndex(e => e.ColorId, "IX_cars_ColorId");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.ColorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CarImage>(entity =>
            {
                entity.ToTable("car_images");

                entity.HasIndex(e => e.CarId, "IX_car_images_CarId");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.CarImages)
                    .HasForeignKey(d => d.CarId);
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.ToTable("colors");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.HasIndex(e => e.UserId, "IX_customers_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OperationClaim>(entity =>
            {
                entity.ToTable("operation_claims");
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.ToTable("rentals");

                entity.HasIndex(e => e.CarId, "IX_rentals_CarId");

                entity.HasIndex(e => e.CustomerId, "IX_rentals_CustomerId");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.ProfileImagePath)
                    .HasMaxLength(500)
                    .HasColumnName("profile_image_path");
            });

            modelBuilder.Entity<UserOperationClaim>(entity =>
            {
                entity.ToTable("user_operation_claims");

                entity.HasIndex(e => e.OperationClaimId, "IX_user_operation_claims_OperationClaimId");

                entity.HasIndex(e => e.UserId, "IX_user_operation_claims_UserId");

                entity.HasOne(d => d.OperationClaim)
                    .WithMany(p => p.UserOperationClaims)
                    .HasForeignKey(d => d.OperationClaimId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserOperationClaims)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
