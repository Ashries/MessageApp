using Microsoft.EntityFrameworkCore;
using MessageApp.Models;

namespace MessageApp.Data
{
    public class MessageContext : DbContext
    {
        public MessageContext(DbContextOptions<MessageContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.JoinDate).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Message entity
            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(m => m.SentAt).HasDefaultValueSql("GETUTCDATE()");

                // Configure relationships
                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(m => m.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.PreviousMessage)
                    .WithMany(m => m.Replies)
                    .HasForeignKey(m => m.PreviousMessageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}