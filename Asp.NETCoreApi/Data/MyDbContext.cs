using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asp.NETCoreApi.Data {
    public class MyDbContext : IdentityDbContext<ApplicationUser> {
        public MyDbContext (DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ToBuyLater> ToBuyLaters { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Deliver> Delivers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }


        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder); // This ensures Identity tables are configured properly

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Colors)
                .WithOne(c => c.Product)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Color>()
                .HasMany(c => c.Sizes)
                .WithOne(s => s.Color)
                .HasForeignKey(s => s.ColorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Color>()
                .HasMany(c => c.Images)
                .WithOne(i => i.Color)
                .HasForeignKey(i => i.ColorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Defining relationships for ToBuyLater

            //modelBuilder.Entity<ToBuyLater>()
            //    .HasOne(t => t.User)
            //    .WithMany() // Assuming ApplicationUser does not have a navigation property back to ToBuyLater
            //    .HasForeignKey(t => t.UserId)
            //    .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as needed
        }
    }
}
