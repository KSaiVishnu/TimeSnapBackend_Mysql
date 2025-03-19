using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TimeSnapBackend_MySql.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<UserEmployee> UserEmployees { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure EmpId is unique in AppUser
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.EmpId)
                .IsUnique();

            // Configure EmpId as a foreign key in Timesheet
            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Employee)
                .WithMany(u => u.Timesheets)
                .HasForeignKey(t => t.EmpId)
                .HasPrincipalKey(u => u.EmpId);

            // Configure TaskId as a foreign key in Timesheet
            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Task)
                .WithMany()
                .HasForeignKey(t => t.TaskId);
        }
    }
}
