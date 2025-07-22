using Microsoft.EntityFrameworkCore;
using AiAgentApi.Models;

namespace AiAgentApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.WhatsApp).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.WhatsApp).IsUnique();
        });

        // Subscription
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PlanName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                  .WithMany(e => e.Subscriptions)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PaymentMethod)
                  .WithMany(e => e.Subscriptions)
                  .HasForeignKey(e => e.PaymentMethodId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaymentId).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PaymentProvider).HasMaxLength(50);
            entity.Property(e => e.PaymentType).HasMaxLength(50);
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Subscription)
                  .WithMany(e => e.Payments)
                  .HasForeignKey(e => e.SubscriptionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PaymentMethod)
                  .WithMany()
                  .HasForeignKey(e => e.PaymentMethodId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.PaymentId);
            entity.HasIndex(e => e.TransactionId);
        });

        // PaymentMethod
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Provider).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ExternalId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Token).HasMaxLength(255);
            entity.Property(e => e.Brand).HasMaxLength(30);
            entity.Property(e => e.Last4).HasMaxLength(4);
            entity.Property(e => e.Type).HasMaxLength(30);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                  .WithMany(e => e.PaymentMethods)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ExternalId).IsUnique();
        });
    }
}
