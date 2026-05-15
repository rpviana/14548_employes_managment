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
        public DbSet<Instrument> Instruments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Liga a duracao da ausencia a um detalhe de codigo do sistema.
            modelBuilder.Entity<LeaveApplication>()
                .HasOne(l => l.Duration)
                .WithMany()
                .HasForeignKey(l => l.DurationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Liga o estado da ausencia ao detalhe do codigo correspondente.
            modelBuilder.Entity<LeaveApplication>()
                .HasOne(l => l.Status)
                .WithMany()
                .HasForeignKey(l => l.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dados base que a app precisa para arrancar sem ficar vazia.
            modelBuilder.Entity<SystemCode>().HasData(
                new SystemCode { Id = 1, Code = "LeaveApprovalStatus", Description = "Leave Approval Status", IsActive = true },
                new SystemCode { Id = 2, Code = "LeaveDuration", Description = "Leave Duration", IsActive = true }
            );

            modelBuilder.Entity<SystemCodeDetail>().HasData(
                // Estados de aprovacao das faltas.
                new SystemCodeDetail { Id = 1, Description = "Pending", SystemCodeId = 1, IsActive = true },
                new SystemCodeDetail { Id = 2, Description = "Awaiting Approval", SystemCodeId = 1, IsActive = true },
                new SystemCodeDetail { Id = 3, Description = "Approved", SystemCodeId = 1, IsActive = true },
                new SystemCodeDetail { Id = 4, Description = "Rejected", SystemCodeId = 1, IsActive = true },
                // Tipos de duracao da falta.
                new SystemCodeDetail { Id = 10, Description = "Full Day", SystemCodeId = 2, IsActive = true },
                new SystemCodeDetail { Id = 11, Description = "First Half", SystemCodeId = 2, IsActive = true },
                new SystemCodeDetail { Id = 12, Description = "Second Half", SystemCodeId = 2, IsActive = true }
            );

            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { Id = 1, Name = "Annual Leave", Description = "Paid annual leave", MaxDaysPerYear = 30, IsActive = true },
                new LeaveType { Id = 2, Name = "Sick Leave", Description = "Sick leave", MaxDaysPerYear = 15, IsActive = true }
            );

            // O empregado de exemplo fica de fora para nao criar conflito de chaves na base já existente.
        }
    }
}
