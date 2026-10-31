using FoodDelivery.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.DAL.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<FoodOrder> FoodOrders => Set<FoodOrder>();
    public DbSet<FoodOrderItem> FoodOrderItems => Set<FoodOrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(200);
        });

        builder.Entity<Restaurant>(entity =>
        {
            entity.Property(r => r.Name).IsRequired().HasMaxLength(200);
            entity.Property(r => r.Description).HasMaxLength(2000);
            entity.Property(r => r.ImageUrl).HasMaxLength(500);
        });

        builder.Entity<Dish>(entity =>
        {
            entity.Property(d => d.Name).IsRequired().HasMaxLength(200);
            entity.Property(d => d.Description).HasMaxLength(2000);
            entity.Property(d => d.Price).HasPrecision(18, 2);
            entity.HasOne(d => d.Restaurant)
                .WithMany(r => r.Dishes)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FoodOrder>(entity =>
        {
            entity.Property(order => order.RestaurantName).IsRequired().HasMaxLength(200);
            entity.Property(order => order.RecipientName).IsRequired().HasMaxLength(200);
            entity.Property(order => order.RecipientEmail).IsRequired().HasMaxLength(256);
            entity.Property(order => order.RecipientPhone).IsRequired().HasMaxLength(40);
            entity.Property(order => order.DeliveryAddress).IsRequired().HasMaxLength(1000);
            entity.Property(order => order.Total).HasPrecision(18, 2);
            entity.HasIndex(order => order.CreatedAtUtc);
            entity.HasOne(order => order.User)
                .WithMany()
                .HasForeignKey(order => order.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(order => order.Restaurant)
                .WithMany()
                .HasForeignKey(order => order.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(order => order.Items)
                .WithOne(item => item.FoodOrder)
                .HasForeignKey(item => item.FoodOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FoodOrderItem>(entity =>
        {
            entity.Property(item => item.DishName).IsRequired().HasMaxLength(200);
            entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
            entity.Property(item => item.LineTotal).HasPrecision(18, 2);
        });
    }
}
