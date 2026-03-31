using GoodHamburger.Shared.Models; 
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<MenuItem>(builder =>
            {
                builder.HasKey(m => m.MenuItemId);
                builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
                builder.Property(m => m.Type).IsRequired().HasConversion<string>();
                builder.Property(m => m.Price).HasColumnType("decimal(18,2)");
            });

            
            modelBuilder.Entity<OrderItem>(builder =>
            {
                builder.HasKey(oi => oi.Id);
                builder.Property(oi => oi.Price).HasColumnType("decimal(18,2)");
                builder.Property(oi => oi.Type).HasConversion<string>();

                
                builder.HasOne<Order>()
                       .WithMany(o => o.OrderItems)
                       .HasForeignKey(oi => oi.OrderId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            
            modelBuilder.Entity<Order>(builder =>
            {
                builder.HasKey(o => o.OrderId);
                builder.Property(o => o.Price).HasColumnType("decimal(18,2)");
                builder.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");
            });

            
            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem { MenuItemId = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"), Name = "X Burger", Price = 5.00m, Type = TypeItem.Hamburger },
                new MenuItem { MenuItemId = Guid.Parse("A2B2C3D4-E5F6-7890-ABCD-EF1234567890"), Name = "X Egg", Price = 4.50m, Type = TypeItem.Hamburger },
                new MenuItem { MenuItemId = Guid.Parse("A3B2C3D4-E5F6-7890-ABCD-EF1234567890"), Name = "X Bacon", Price = 7.00m, Type = TypeItem.Hamburger },
                new MenuItem { MenuItemId = Guid.Parse("A4B2C3D4-E5F6-7890-ABCD-EF1234567890"), Name = "Fries", Price = 2.00m, Type = TypeItem.Extra },
                new MenuItem { MenuItemId = Guid.Parse("A5B2C3D4-E5F6-7890-ABCD-EF1234567890"), Name = "Soft Drink", Price = 2.50m, Type = TypeItem.Extra }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}