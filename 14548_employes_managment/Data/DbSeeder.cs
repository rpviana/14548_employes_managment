using _14548_employes_managment.Models;
using Microsoft.EntityFrameworkCore;

namespace _14548_employes_managment.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Garante que a base existe antes de tentar inserir dados.
            await context.Database.EnsureCreatedAsync();

            // Garante os codigos base usados pela aplicacao.
            if (!await context.SystemCodes.AnyAsync(sc => sc.Code == "LeaveApprovalStatus"))
            {
                var sc = new SystemCode { Code = "LeaveApprovalStatus", Description = "Leave Approval Status", IsActive = true };
                context.SystemCodes.Add(sc);
                await context.SaveChangesAsync();

                var pending = new SystemCodeDetail { Description = "Pending", SystemCodeId = sc.Id, IsActive = true };
                var awaiting = new SystemCodeDetail { Description = "Awaiting Approval", SystemCodeId = sc.Id, IsActive = true };
                var approved = new SystemCodeDetail { Description = "Approved", SystemCodeId = sc.Id, IsActive = true };
                var rejected = new SystemCodeDetail { Description = "Rejected", SystemCodeId = sc.Id, IsActive = true };
                context.SystemCodeDetails.AddRange(pending, awaiting, approved, rejected);
                await context.SaveChangesAsync();
            }

            if (!await context.SystemCodes.AnyAsync(sc => sc.Code == "LeaveDuration"))
            {
                var sc = new SystemCode { Code = "LeaveDuration", Description = "Leave Duration", IsActive = true };
                context.SystemCodes.Add(sc);
                await context.SaveChangesAsync();

                var full = new SystemCodeDetail { Description = "Full Day", SystemCodeId = sc.Id, IsActive = true };
                var first = new SystemCodeDetail { Description = "First Half", SystemCodeId = sc.Id, IsActive = true };
                var second = new SystemCodeDetail { Description = "Second Half", SystemCodeId = sc.Id, IsActive = true };
                context.SystemCodeDetails.AddRange(full, first, second);
                await context.SaveChangesAsync();
            }

            // Tipos de ausencia iniciais.
            if (!await context.LeaveTypes.AnyAsync())
            {
                context.LeaveTypes.AddRange(
                    new LeaveType { Name = "Annual Leave", Description = "Paid annual leave", MaxDaysPerYear = 30, IsActive = true },
                    new LeaveType { Name = "Sick Leave", Description = "Sick leave", MaxDaysPerYear = 15, IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Deixa um empregado de exemplo para facilitar testes locais.
            if (!await context.Employees.AnyAsync())
            {
                _ = context.Employees.Add(new Employee
                {
                    EmpNo = "EMP001",
                    FirstName = "João",
                    MiddleName = string.Empty,
                    LastName = "Silva",
                    PhoneNumber = "+351912345678",
                    EmailAddress = "joao.silva@example.com",
                    Country = "Portugal",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Address = "Rua Exemplo 1",
                    Department = "HR",
                    Designation = "Analyst",
                    CreatedById = "seed",
                    CreatedAt = DateTime.Now,
                    ModifiedById = "seed",
                    ModifiedAt = DateTime.Now
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
