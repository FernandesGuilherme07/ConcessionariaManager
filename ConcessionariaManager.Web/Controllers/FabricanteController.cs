using Microsoft.AspNetCore.Mvc;
using ConcessionariaManager.Core.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using ConcessionariaManager.Core.Interfaces.Repositories;

namespace ConcessionariaManager.Web.Controllers
{
    public class FabricanteController : Controller
    {
        private readonly IFabricanteRepository _repository;

        public FabricanteController(IFabricanteRepository repository)
        {
            _repository = repository;
        }
        [Authorize]
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var query = _repository.GetAll(searchString).AsQueryable();

            ViewData["searchString"] = searchString;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedList = query.ToPagedList(pageNumber, pageSize);
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

            var fabricante = await _repository.GetByIdAsync(id);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Nome,PaisOrigem,AnoFundacao,Website,Id")] Fabricante fabricante)
        {
            if (ModelState.IsValid)
            {
                if (await _repository.ExistsByNameAsync(fabricante.Nome))
                {
                    ModelState.AddModelError("Nome", "O Nome já está em uso.");
                    return View(fabricante);
                }
                await _repository.AddAsync(fabricante);
                TempData["Sucesso"] = "Fábricante criado com sucesso!";
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

            var fabricante = await _repository.GetByIdAsync(id);
            if (fabricante == null)
            {
                return NotFound();
            }

            if (await _repository.ExistsByNameAsync(fabricante.Nome, id))
            {
                ModelState.AddModelError("Nome", "O Nome já está em uso.");
                return View(fabricante);
            }

            return View(fabricante);
        }

        // POST: Fabricante/Edit/5
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

                if (await _repository.ExistsByNameAsync(fabricante.Nome, id))
                {
                    ModelState.AddModelError("Nome", "O Nome já está em uso.");
                    return View(fabricante);
                }
                var fabricanteEditado = await _repository.GetByIdAsync(id);
                fabricanteEditado.Nome = fabricante.Nome;
                fabricanteEditado.PaisOrigem = fabricante.PaisOrigem;
                fabricanteEditado.AnoFundacao = fabricante.AnoFundacao;
                fabricanteEditado.Website = fabricante.Website;
                fabricanteEditado.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(fabricanteEditado);
                TempData["Successo"] = "Fábricante atualizada com sucesso!";

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

            var fabricante = await _repository.GetByIdAsync(id);
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
            var fabricante = await _repository.GetByIdAsync(id);
            if (fabricante != null)
            {
                await _repository.SoftDeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
