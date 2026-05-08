using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using _14548_employes_managment.Models;

namespace _14548_employes_managment.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<SystemCode> SystemCodes { get; set; }
        public DbSet<SystemCodeDetail> SystemCodeDetails { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relacionamento LeaveApplication -> SystemCodeDetail (Duration)
            modelBuilder.Entity<LeaveApplication>()
                .HasOne(l => l.Duration)
                .WithMany()
                .HasForeignKey(l => l.DurationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relacionamento LeaveApplication -> SystemCodeDetail (Status)
            modelBuilder.Entity<LeaveApplication>()
                .HasOne(l => l.Status)
                .WithMany()
                .HasForeignKey(l => l.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data for SystemCodes, SystemCodeDetails, LeaveTypes and a sample Employee
            modelBuilder.Entity<SystemCode>().HasData(
                new SystemCode { Id = 1, Code = "LeaveApprovalStatus", Description = "Leave Approval Status", IsActive = true },
                new SystemCode { Id = 2, Code = "LeaveDuration", Description = "Leave Duration", IsActive = true }
            );

            modelBuilder.Entity<SystemCodeDetail>().HasData(
                // LeaveApprovalStatus details
                new SystemCodeDetail { Id = 1, Description = "Pending", SystemCodeId = 1, IsActive = true },
                new SystemCodeDetail { Id = 2, Description = "Awaiting Approval", SystemCodeId = 1, IsActive = true },
                new SystemCodeDetail { Id = 3, Description = "Approved", SystemCodeId = 1, IsActive = true },
                new SystemCodeDetail { Id = 4, Description = "Rejected", SystemCodeId = 1, IsActive = true },
                // LeaveDuration details
                new SystemCodeDetail { Id = 10, Description = "Full Day", SystemCodeId = 2, IsActive = true },
                new SystemCodeDetail { Id = 11, Description = "First Half", SystemCodeId = 2, IsActive = true },
                new SystemCodeDetail { Id = 12, Description = "Second Half", SystemCodeId = 2, IsActive = true }
            );

            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { Id = 1, Name = "Annual Leave", Description = "Paid annual leave", MaxDaysPerYear = 30, IsActive = true },
                new LeaveType { Id = 2, Name = "Sick Leave", Description = "Sick leave", MaxDaysPerYear = 15, IsActive = true }
            );

            // Note: sample Employee not seeded to avoid primary key conflicts on existing DB
        }
    }
}
