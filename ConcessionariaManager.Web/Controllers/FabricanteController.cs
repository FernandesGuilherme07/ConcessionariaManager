using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Web.Data;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;

namespace ConcessionariaManager.Web.Controllers
{
    public class FabricanteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FabricanteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fabricante
        [Authorize]
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var query = _context.Fabricantes.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(f => f.Nome.Contains(searchString));
            }

            ViewData["searchString"] = searchString;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedList = query.OrderByDescending(f => f.Nome).ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        // GET: Fabricante/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fabricante = await _context.Fabricantes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fabricante == null)
            {
                return NotFound();
            }

            return View(fabricante);
        }

        // GET: Fabricante/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fabricante/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Nome,PaisOrigem,AnoFundacao,Website,Id")] Fabricante fabricante)
        {
            if (ModelState.IsValid)
            {
                if (_context.Fabricantes.Any(c => c.Nome == fabricante.Nome))
                {
                    ModelState.AddModelError("Nome", "O Nome já está em uso.");
                    return View(fabricante);
                }
                _context.Add(fabricante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fabricante);
        }

        // GET: Fabricante/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fabricante = await _context.Fabricantes.FindAsync(id);
            if (fabricante == null)
            {
                return NotFound();
            }

            if (_context.Fabricantes.Any(c => c.Nome == fabricante.Nome && c.Id != id))
            {
                ModelState.AddModelError("Nome", "O Nome já está em uso.");
                return View(fabricante);
            }

            return View(fabricante);
        }

        // POST: Fabricante/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Nome,PaisOrigem,AnoFundacao,Website,Id")] Fabricante fabricante)
        {
            if (id != fabricante.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (_context.Fabricantes.Any(c => c.Nome == fabricante.Nome && c.Id != id))
                {
                    ModelState.AddModelError("Nome", "O Nome já está em uso.");
                    return View(fabricante);
                }
                fabricante.UpdatedAt = DateTime.Now;

                _context.Update(fabricante);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(fabricante);
        }

        // GET: Fabricante/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fabricante = await _context.Fabricantes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fabricante == null)
            {
                return NotFound();
            }

            return View(fabricante);
        }

        // POST: Fabricante/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);
            if (fabricante != null)
            {
                fabricante.IsDeleted = true;
                _context.Fabricantes.Update(fabricante);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FabricanteExists(int id)
        {
            return _context.Fabricantes.Any(e => e.Id == id);
        }
    }
}
