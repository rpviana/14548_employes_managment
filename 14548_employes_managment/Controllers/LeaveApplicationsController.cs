using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _14548_employes_managment.Data;
using _14548_employes_managment.Models;

namespace _14548_employes_managment.Controllers
{
    public class LeaveApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LeaveApplications
        public async Task<IActionResult> Index()
        {
            var leaveApplications = await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Duration)
                .Include(l => l.Status)
                .ToListAsync();

            return View(leaveApplications);
        }

        // GET: LeaveApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Duration)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveApplication == null)
                return NotFound();

            return View(leaveApplication);
        }

        // GET: LeaveApplications/Create
        public async Task<IActionResult> Create()
        {
            // Carregar dados para os dropdowns
            ViewData["EmployeeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Employees.ToListAsync(), "Id", "FullName");

            ViewData["LeaveTypeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.LeaveTypes.Where(l => l.IsActive).ToListAsync(), "Id", "Name");

            // Filtrar apenas as durações (LeaveDuration)
            var durationCodes = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveDuration" && y.IsActive)
                .ToListAsync();

            ViewData["DurationId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                durationCodes, "Id", "Description");

            return View();
        }

        // POST: LeaveApplications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,StartDate,EndDate,DurationId,LeaveTypeId,Description")] LeaveApplication leaveApplication)
        {
            try
            {
                // Validar que a data de fim é igual ou posterior à data de início
                if (leaveApplication.EndDate < leaveApplication.StartDate)
                {
                    ModelState.AddModelError("EndDate", "End date must be equal to or later than start date.");
                }

                ApplyCalculatedDays(leaveApplication);

                // Log validation errors
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"ModelState Error: {error.ErrorMessage}");
                    }
                }

                if (ModelState.IsValid)
                {
                    // Buscar o status "Pending" automaticamente
                    var pendingStatus = await _context.SystemCodeDetails
                        .Include(x => x.SystemCode)
                        .FirstOrDefaultAsync(y => y.Description == "Pending" && y.SystemCode.Code == "LeaveApprovalStatus");

                    if (pendingStatus == null)
                    {
                        ModelState.AddModelError(string.Empty, "System error: Cannot find 'Pending' status. Please contact administrator.");
                        throw new InvalidOperationException("Pending status not found in SystemCodeDetails");
                    }

                    leaveApplication.StatusId = pendingStatus.Id;

                    // Atribuir dados de auditoria
                    leaveApplication.CreatedById = "MacroCode"; // TODO: Usar User.Identity.Name quando autenticação estiver pronta
                    leaveApplication.CreatedAt = DateTime.Now;

                    _context.Add(leaveApplication);
                    await _context.SaveChangesAsync();
                    
                    System.Diagnostics.Debug.WriteLine($"Leave application created successfully with ID: {leaveApplication.Id}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in Create: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            // Recarregar dados se houver erro
            ViewData["EmployeeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Employees.ToListAsync(), "Id", "FullName", leaveApplication.EmployeeId);

            ViewData["LeaveTypeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.LeaveTypes.Where(l => l.IsActive).ToListAsync(), "Id", "Name", leaveApplication.LeaveTypeId);

            var durationCodes = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveDuration" && y.IsActive)
                .ToListAsync();

            ViewData["DurationId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                durationCodes, "Id", "Description", leaveApplication.DurationId);

            return View(leaveApplication);
        }

        // GET: LeaveApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var leaveApplication = await _context.LeaveApplications.FindAsync(id);
            if (leaveApplication == null)
                return NotFound();

            // Carregar dados para os dropdowns
            ViewData["EmployeeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Employees.ToListAsync(), "Id", "FullName", leaveApplication.EmployeeId);

            ViewData["LeaveTypeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.LeaveTypes.Where(l => l.IsActive).ToListAsync(), "Id", "Name", leaveApplication.LeaveTypeId);

            var durationCodes = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveDuration" && y.IsActive)
                .ToListAsync();

            ViewData["DurationId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                durationCodes, "Id", "Description", leaveApplication.DurationId);

            return View(leaveApplication);
        }

        // POST: LeaveApplications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,StartDate,EndDate,DurationId,LeaveTypeId,Description")] LeaveApplication leaveApplication)
        {
            if (id != leaveApplication.Id)
                return NotFound();

            // Validar que a data de fim é igual ou posterior à data de início
            if (leaveApplication.EndDate < leaveApplication.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be equal to or later than start date.");
            }

            ApplyCalculatedDays(leaveApplication);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingApplication = await _context.LeaveApplications.FindAsync(id);
                    if (existingApplication != null)
                    {
                        existingApplication.EmployeeId = leaveApplication.EmployeeId;
                        existingApplication.StartDate = leaveApplication.StartDate;
                        existingApplication.EndDate = leaveApplication.EndDate;
                        existingApplication.NumberOfDays = leaveApplication.NumberOfDays;
                        existingApplication.DurationId = leaveApplication.DurationId;
                        existingApplication.LeaveTypeId = leaveApplication.LeaveTypeId;
                        existingApplication.Description = leaveApplication.Description;
                        existingApplication.ModifiedById = "MacroCode"; // TODO: Usar User.Identity.Name
                        existingApplication.ModifiedAt = DateTime.Now;

                        _context.Update(existingApplication);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveApplicationExists(leaveApplication.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // Recarregar dados se houver erro
            ViewData["EmployeeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Employees.ToListAsync(), "Id", "FullName", leaveApplication.EmployeeId);

            ViewData["LeaveTypeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.LeaveTypes.Where(l => l.IsActive).ToListAsync(), "Id", "Name", leaveApplication.LeaveTypeId);

            var durationCodes = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveDuration" && y.IsActive)
                .ToListAsync();

            ViewData["DurationId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                durationCodes, "Id", "Description", leaveApplication.DurationId);

            return View(leaveApplication);
        }

        // GET: LeaveApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveApplication == null)
                return NotFound();

            return View(leaveApplication);
        }

        // POST: LeaveApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveApplication = await _context.LeaveApplications.FindAsync(id);
            if (leaveApplication != null)
            {
                _context.LeaveApplications.Remove(leaveApplication);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LeaveApplicationExists(int id)
        {
            return _context.LeaveApplications.Any(e => e.Id == id);
        }

        private void ApplyCalculatedDays(LeaveApplication leaveApplication)
        {
            if (leaveApplication.StartDate == default || leaveApplication.EndDate == default)
            {
                ModelState.AddModelError(string.Empty, "Start Date and End Date are required.");
                return;
            }

            if (leaveApplication.EndDate < leaveApplication.StartDate)
            {
                ModelState.AddModelError(nameof(leaveApplication.EndDate), "End Date must be on or after Start Date.");
                return;
            }

            leaveApplication.NumberOfDays = (leaveApplication.EndDate.Date - leaveApplication.StartDate.Date).Days + 1;
        }
    }
}
