
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _14548_employes_managment.Data;
using _14548_employes_managment.Models;

namespace _14548_employes_managment.Controllers
{
    public class InstrumentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstrumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista os Instumentos que estao gravados na base de dados.
        public async Task<IActionResult> Index()
        {
            return View(await _context.Instruments.ToListAsync());
        }

        // Mostra os detalhes de um instrumento especifico.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instruments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instrument == null)
            {
                return NotFound();
            }

            return View(instrument);
        }

        // Abre o formulario para criar um instrumento.
        public IActionResult Create()
        {
            return View();
        }

        // Recebe o formulario e grava o novo instrumento.
        // Aqui só se ligam os campos que o formulario realmente envia.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Instrument instrument)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instrument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instrument);
        }

        // Abre o formulario de edicao com os dados do instrumento.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instruments.FindAsync(id);
            if (instrument == null)
            {
                return NotFound();
            }
            return View(instrument);
        }

        // Recebe a edicao e actualiza o registo.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Instrument instrument)
        {
            if (id != instrument.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instrument);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstrumentExists(instrument.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instrument);
        }

        // Mostra a confirmacao antes de apagar.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instruments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instrument == null)
            {
                return NotFound();
            }

            return View(instrument);
        }

        // Apaga o registo depois da confirmacao.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instrument = await _context.Instruments.FindAsync(id);
            if (instrument != null)
            {
                _context.Instruments.Remove(instrument);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Confirma se o instrumento ainda existe na base.
        private bool InstrumentExists(int id)
        {
            return _context.Instruments.Any(i => i.Id == id);
        }
    }
}
