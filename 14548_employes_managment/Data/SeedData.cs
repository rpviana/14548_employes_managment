using _14548_employes_managment.Data;
using _14548_employes_managment.Models;
using Microsoft.EntityFrameworkCore;

namespace _14548_employes_managment.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Checar se já existem dados
            if (context.SystemCodes.Any())
                return;

            var systemCodes = new List<SystemCode>
            {
                new SystemCode { Code = "LeaveApprovalStatus", Description = "Leave Approval Status", IsActive = true },
                new SystemCode { Code = "LeaveDuration", Description = "Leave Duration", IsActive = true }
            };

            context.SystemCodes.AddRange(systemCodes);
            context.SaveChanges();

            // Buscar os SystemCodes que acabamos de criar
            var leaveApprovalStatusCode = context.SystemCodes.FirstOrDefault(x => x.Code == "LeaveApprovalStatus");
            var leaveDurationCode = context.SystemCodes.FirstOrDefault(x => x.Code == "LeaveDuration");

            var systemCodeDetails = new List<SystemCodeDetail>();

            // Adicionar detalhes para LeaveApprovalStatus
            if (leaveApprovalStatusCode != null)
            {
                systemCodeDetails.AddRange(new[]
                {
                    new SystemCodeDetail { Description = "Pending", SystemCodeId = leaveApprovalStatusCode.Id, IsActive = true },
                    new SystemCodeDetail { Description = "Awaiting Approval", SystemCodeId = leaveApprovalStatusCode.Id, IsActive = true },
                    new SystemCodeDetail { Description = "Approved", SystemCodeId = leaveApprovalStatusCode.Id, IsActive = true },
                    new SystemCodeDetail { Description = "Rejected", SystemCodeId = leaveApprovalStatusCode.Id, IsActive = true }
                });
            }

            // Adicionar detalhes para LeaveDuration
            if (leaveDurationCode != null)
            {
                systemCodeDetails.AddRange(new[]
                {
                    new SystemCodeDetail { Description = "Full Day", SystemCodeId = leaveDurationCode.Id, IsActive = true },
                    new SystemCodeDetail { Description = "First Half", SystemCodeId = leaveDurationCode.Id, IsActive = true },
                    new SystemCodeDetail { Description = "Second Half", SystemCodeId = leaveDurationCode.Id, IsActive = true }
                });
            }

            context.SystemCodeDetails.AddRange(systemCodeDetails);
            context.SaveChanges();

            // Adicionar tipos de licença
            var leaveTypes = new List<LeaveType>
            {
                new LeaveType { Name = "Annual Leave", Description = "Yearly vacation days", MaxDaysPerYear = 20, IsActive = true },
                new LeaveType { Name = "Sick Leave", Description = "Medical leave", MaxDaysPerYear = 10, IsActive = true },
                new LeaveType { Name = "Casual Leave", Description = "Casual leave without pay", MaxDaysPerYear = 5, IsActive = true },
                new LeaveType { Name = "Maternity Leave", Description = "Maternity leave", MaxDaysPerYear = 90, IsActive = true },
                new LeaveType { Name = "Paternity Leave", Description = "Paternity leave", MaxDaysPerYear = 10, IsActive = true }
            };

            context.LeaveTypes.AddRange(leaveTypes);
            context.SaveChanges();
        }
    }
}
