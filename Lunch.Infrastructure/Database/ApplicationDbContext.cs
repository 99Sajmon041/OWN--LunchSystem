using Lunch.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lunch.Infrastructure.Database
{
    public class ApplicationDbContext : IdentityDbContext<Customer>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Meal> Meals => Set<Meal>();
        public DbSet<PaidFood> PaidFoods => Set<PaidFood>();
        public DbSet<WrittenOffFood> WrittenOffFoods => Set<WrittenOffFood>();
        public DbSet<RoleDiscount> RoleDiscounts => Set<RoleDiscount>();
        public DbSet<DailyOffer> DailyOffers => Set<DailyOffer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DailyOffer>().ToTable("DailyOffer");

            modelBuilder.Entity<Customer>()
                .Property(c => c.MustChangePassword)
                .HasDefaultValue(true);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Credit)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            modelBuilder.Entity<RoleDiscount>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(rd => rd.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoleDiscount>()
                .Property(r => r.Discount)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<RoleDiscount>()
                .HasIndex(rd => rd.RoleId)
                .IsUnique();

            modelBuilder.Entity<PaidFood>()
                .HasIndex(p => new { p.CustomerId, p.DailyOfferId })
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CardId)
                .IsUnique();
        }
    }
}
