//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;

//namespace TimeSnapBackend_MySql.Models
//{
//    public class AppDbContext : IdentityDbContext<AppUser>
//    {
//        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//        public DbSet<AppUser> AppUsers { get; set; }
//        public DbSet<TaskModel> Tasks { get; set; }
//        public DbSet<UserEmployee> UserEmployees { get; set; }
//        public DbSet<Timesheet> Timesheets { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            // Ensure EmpId is unique in AppUser
//            modelBuilder.Entity<AppUser>()
//                .HasIndex(u => u.EmpId)
//                .IsUnique();

//            // Configure EmpId as a foreign key in Timesheet
//            modelBuilder.Entity<Timesheet>()
//                .HasOne(t => t.Employee)
//                .WithMany(u => u.Timesheets)
//                .HasForeignKey(t => t.EmpId)
//                .HasPrincipalKey(u => u.EmpId);

//            // Configure TaskId as a foreign key in Timesheet
//            modelBuilder.Entity<Timesheet>()
//                .HasOne(t => t.Task)
//                .WithMany()
//                .HasForeignKey(t => t.TaskId);
//        }
//    }
//}




using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TimeSnapBackend_MySql.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<UserEmployee> UserEmployees { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure EmpId is unique in AppUser
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.EmpId)
                .IsUnique();

            modelBuilder.Entity<TaskModel>()
                .HasIndex(t => t.TaskId)
                .IsUnique();

            modelBuilder.Entity<UserEmployee>()
                .HasIndex(ue => ue.EmployeeId)
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
                .HasForeignKey(t => t.TaskId)
                .HasPrincipalKey(ta => ta.TaskId); // Both should be strings now


            // Configure UserTask (Many-to-Many Relationship)
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.Task)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(ut => ut.TaskId)
                .HasPrincipalKey(t => t.TaskId); // Use TaskId as reference


            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.Employee)
                .WithMany()
                .HasForeignKey(ut => ut.EmpId)
                .HasPrincipalKey(u => u.EmpId);
        }
    }
}

