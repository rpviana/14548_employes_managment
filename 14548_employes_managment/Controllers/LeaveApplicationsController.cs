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

        // Lista os pedidos de ausencia com os dados relacionados.
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

        // Mostra os detalhes de um pedido especifico.
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

        // Abre o formulario para criar um pedido de ausencia.
        public async Task<IActionResult> Create()
        {
            // Carrega os dados para os campos de escolha.
            ViewData["EmployeeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Employees.ToListAsync(), "Id", "FullName");

            ViewData["LeaveTypeId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.LeaveTypes.Where(l => l.IsActive).ToListAsync(), "Id", "Name");

            // Usa apenas as duracoes ligadas ao codigo LeaveDuration.
            var durationCodes = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveDuration" && y.IsActive)
                .ToListAsync();

            ViewData["DurationId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                durationCodes, "Id", "Description");

            return View();
        }

        // Recebe o formulario e grava o pedido.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,StartDate,EndDate,DurationId,LeaveTypeId,Description")] LeaveApplication leaveApplication)
        {
            try
            {
                // Garante que a data final nao fica antes da data inicial.
                if (leaveApplication.EndDate < leaveApplication.StartDate)
                {
                    ModelState.AddModelError("EndDate", "End date must be equal to or later than start date.");
                }

                ApplyCalculatedDays(leaveApplication);

                // Regista os erros de validacao no debug para facilitar o teste.
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
                    // Procura automaticamente o estado Pending.
                    var pendingStatus = await _context.SystemCodeDetails
                        .Include(x => x.SystemCode)
                        .FirstOrDefaultAsync(y => y.Description == "Pending" && y.SystemCode.Code == "LeaveApprovalStatus");

                    if (pendingStatus == null)
                    {
                        ModelState.AddModelError(string.Empty, "System error: Cannot find 'Pending' status. Please contact administrator.");
                        throw new InvalidOperationException("Pending status not found in SystemCodeDetails");
                    }

                    leaveApplication.StatusId = pendingStatus.Id;

                    // Dados de auditoria enquanto a autenticacao final nao esta ligada.
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

            // Recarrega os campos de escolha se algo correr mal.
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

        // Abre o formulario de edicao com os dados atuais.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var leaveApplication = await _context.LeaveApplications.FindAsync(id);
            if (leaveApplication == null)
                return NotFound();

            // Carrega os dados para os campos de escolha.
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

        // Recebe a edicao e atualiza o pedido.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,StartDate,EndDate,DurationId,LeaveTypeId,Description")] LeaveApplication leaveApplication)
        {
            if (id != leaveApplication.Id)
                return NotFound();

            // Garante que a data final nao fica antes da data inicial.
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

            // Recarrega os campos de escolha se houver erro.
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

        // Mostra a confirmacao antes de apagar.
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

        // Apaga o pedido depois da confirmacao.
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

        // Confirma se o pedido ainda existe.
        private bool LeaveApplicationExists(int id)
        {
            return _context.LeaveApplications.Any(e => e.Id == id);
        }

        // Calcula os dias da ausencia com base nas datas escolhidas.
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
