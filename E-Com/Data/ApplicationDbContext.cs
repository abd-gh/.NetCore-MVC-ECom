using E_Com.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Com.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        
     
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Payment>()
                 .HasOne(b => b.Cart)
                 .WithMany(ba => ba.Payments)
                 .HasForeignKey(bi => bi.cartId);

           builder.Entity<Payment>()
                .HasOne(b => b.User)
                .WithMany(ba => ba.Payments)
                .HasForeignKey(bi => bi.UserId);

            builder.Entity<Payment>()
                .HasOne(b => b.BillingAddress)
                .WithMany(ba => ba.Payments)
                .HasForeignKey(bi => bi.billingId);

            base.OnModelCreating(builder);
        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BillingAddress> BillingAddresses { get; set; }
    }
}