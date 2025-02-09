using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    public DbSet<Shop> Shops => Set<Shop>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<User> Users => Set<User>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    private const string DecimalType = "decimal(8, 2)";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<UserRole>()
            .HasIndex(r => r.Name)
            .IsUnique();

        modelBuilder.Entity<ProductCategory>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name)
            .IsUnique();
        
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType(DecimalType);
        
        modelBuilder.Entity<Product>()
            .Property(p => p.Weight)
            .HasColumnType(DecimalType);
        
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalOrderPrice)
            .HasColumnType(DecimalType);
            

        modelBuilder.Entity<OrderLine>()
            .HasIndex(ol => new { ol.OrderId, ol.ProductId })
            .IsUnique();
        
        modelBuilder.Entity<OrderLine>()
            .Property(ol => ol.TotalLinePrice)
            .HasColumnType(DecimalType);

        modelBuilder.Entity<Shop>()
            .HasIndex(s => new { s.Street, s.Building })
            .IsUnique();

        modelBuilder.Entity<UserRole>()
            .HasMany(r => r.Users)
            .WithOne(u => u.UserRole)
            .HasForeignKey(u => u.UserRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderLines)
            .WithOne(ol => ol.Product)
            .HasForeignKey(ol => ol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductCategory>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderLines)
            .WithOne(ol => ol.Order)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Shop>()
            .HasMany(s => s.Orders)
            .WithOne(o => o.Shop)
            .HasForeignKey(o => o.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
