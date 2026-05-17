using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FerienspassWebApp.Models;

namespace FerienspassWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Child> Children { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventChild> EventChildren { get; set; }

        public DbSet<EventRoleAssignment> EventRoleAssignments { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        public DbSet<EventKursleiter> EventKursleiter { get; set; }

        public DbSet<SystemSettings> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EmergencyContact>()
                .HasOne(equals => equals.ApplicationUser)
                .WithMany(u => u.Contacts)
                .HasForeignKey(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventChild>()
                .HasIndex(ec => new { ec.EventId, ec.ChildId })
                .IsUnique();

            builder.Entity<Invoice>()
                .HasOne(x => x.ParentUser)
                .WithMany()
                .HasForeignKey(x => x.ParentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EventKursleiter>()
                .HasKey(x => new { x.EventId, x.UserId });

            builder.Entity<EventKursleiter>()
                .HasOne(x => x.Event)
                .WithMany(x => x.Kursleiter)
                .HasForeignKey(x => x.EventId);

            builder.Entity<EventKursleiter>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder.Entity<EventRoleAssignment>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

        }
    }
}
