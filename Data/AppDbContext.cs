using CalendarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        { }

        public DbSet<User>? Users { get; set; }
        public DbSet<Event>? Events { get; set; }
        public DbSet<Reminder>? Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Events)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Reminders)
                .WithOne(r => r.Event)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasMaxLength(36)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Event>()
                .Property(e => e.Id)
                .HasMaxLength(36)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.UserId)
                .HasMaxLength(36)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.GuestsEmailsJson)
                .HasColumnName("GuestsEmails");

            modelBuilder.Entity<Reminder>()
                .Property(r => r.Id)
                .HasMaxLength(36)
                .IsRequired();

            modelBuilder.Entity<Reminder>()
                .Property(r => r.EventId)
                .HasMaxLength(36)
                .IsRequired();

            modelBuilder.Entity<Reminder>()
                .Property(r => r.SendedEmail)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
