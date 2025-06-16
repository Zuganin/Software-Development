using Microsoft.EntityFrameworkCore;
using PaymentsService.Domain.Model.Entities;
using PaymentsService.Domain.Model.Interfaces;
using PaymentsService.Infrastructure.DbContext;
using PaymentsService.Domain.Events; // Для InboxEvent


namespace PaymentsService.Infrastructure.DbContext;

public class PaymentsDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<OutboxEvent> OutboxEvents { get; set; } = null!;
    public DbSet<InboxEvent> InboxEvents { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация агрегата Account
        modelBuilder.Entity<Account>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.UserId)
                .IsRequired();

            builder.Property(a => a.Balance)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(a => a.IsActive)
                .IsRequired();
        });
        modelBuilder.Entity<OutboxEvent>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.EventType).IsRequired();
            builder.Property(e => e.Payload).IsRequired();
            builder.Property(e => e.OccurredOn).IsRequired();
        });
        modelBuilder.Entity<InboxEvent>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.EventType).IsRequired();
            builder.Property(e => e.Payload).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.ReceivedAt).IsRequired();
        });
        modelBuilder.Entity<Transaction>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.AccountId).IsRequired();
            builder.Property(t => t.Amount).HasPrecision(18, 2).IsRequired();
            builder.Property(t => t.OccurredOn).IsRequired();
            builder.Property(t => t.Type).IsRequired();
            builder.Property(t => t.IdempotencyKey).HasMaxLength(100);
        });
        base.OnModelCreating(modelBuilder);
    }
}