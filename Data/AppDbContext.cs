using System.Collections.Generic;
using System.Reflection.Emit;
using ShopZone.Models;
using Microsoft.EntityFrameworkCore;
using ShopZone.Models;

namespace ShopZone.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<OtpRequest> OtpRequests { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Phone).IsUnique();
                //entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // Product Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasOne(d => d.Seller)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SellerId);
            });

            // Cart Configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasOne(d => d.Buyer)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.BuyerId);
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId);
            });

            // DeliveryAddress Configuration
            modelBuilder.Entity<DeliveryAddress>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.DeliveryAddresses)
                    .HasForeignKey(d => d.UserId);
            });

            // Order Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.HasOne(d => d.Buyer)
                    .WithMany(p => p.BuyerOrders)
                    .HasForeignKey(d => d.BuyerId);
                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AddressId);
                entity.HasOne(d => d.DeliveryPartner)
                    .WithMany(p => p.DeliveryPartnerOrders)
                    .HasForeignKey(d => d.DeliveryPartnerId)
                    .IsRequired(false);
            });

            // OrderItem Configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId);
            });

            // Payment Configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasOne(d => d.Order)
                    .WithOne(p => p.Payment)
                    .HasForeignKey<Payment>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RefreshToken Configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId);
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // You can add seed data here if needed
        }
    }
}
