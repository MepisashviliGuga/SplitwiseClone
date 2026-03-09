using Microsoft.EntityFrameworkCore;
using SplitwiseClone.Domain.Entities;

namespace SplitwiseClone.Persistence.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<LedgerEntry> LedgerEntries { get; set; }

        public DbSet<GroupMember> GroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Group>().ToTable("Groups");

            // Fix the decimal warnings
            modelBuilder.Entity<Expense>()
                .ToTable("Expenses")
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 2); // 18 digits total, 2 digits after the decimal (cents)

            modelBuilder.Entity<LedgerEntry>()
                .ToTable("LedgerEntries")
                .Property(le => le.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<GroupMember>()
                .ToTable("GroupMembers")
                .HasKey(gm => new { gm.GroupId, gm.UserId });
        }
    }
}