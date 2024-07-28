using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class StoreDbContext : IdentityDbContext<ApplicationUser>
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    public DbSet<Shop> Shops => Set<Shop>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();

        builder.Entity<ProductCategory>()
            .HasIndex(c => c.Name)
            .IsUnique();

        builder.Entity<Product>()
            .HasIndex(p => p.Name)
            .IsUnique();

        builder.Entity<OrderLine>()
            .HasIndex(ol => new { ol.OrderId, ol.ProductId })
            .IsUnique();

        builder.Entity<Shop>()
            .HasIndex(s => new { s.Street, s.Building })
            .IsUnique();

        builder.Entity<Product>()
            .HasMany(p => p.OrderLines)
            .WithOne(ol => ol.Product)
            .HasForeignKey(ol => ol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProductCategory>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Order>()
            .HasMany(o => o.OrderLines)
            .WithOne(ol => ol.Order)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Shop>()
            .HasMany(s => s.Orders)
            .WithOne(o => o.Shop)
            .HasForeignKey(o => o.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
