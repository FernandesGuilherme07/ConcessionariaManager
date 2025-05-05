using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Web.Data;
using ConcessionariaManager.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using X.PagedList.Extensions;

namespace ConcessionariaManager.Web.Controllers
{
    public class VeiculoController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public VeiculoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Veiculo
        [Authorize]
        public async Task<IActionResult> Index(string searchString, TipoVeiculo tipoVeiculo, int? fabricanteId, int? anoFabricacao, int? page)
        {
            var query = _context.Veiculos
                .Include(v => v.Fabricante)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
                query = query.Where(v => v.Modelo.Contains(searchString));

            if (!string.IsNullOrWhiteSpace(tipoVeiculo.ToString()))
                query = query.Where(v => v.TipoVeiculo == tipoVeiculo);

            if (fabricanteId.HasValue)
                query = query.Where(v => v.FabricanteId == fabricanteId);
            if (anoFabricacao.HasValue)
                query = query.Where(v => v.AnoFabricacao == anoFabricacao);

            ViewData["searchString"] = searchString;
            ViewData["tipoVeiculo"] = tipoVeiculo;
            ViewData["fabricanteId"] = fabricanteId;
            ViewData["anoFabricacao"] = anoFabricacao;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedList = query.OrderBy(v => v.Modelo).ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }



        // GET: Veiculo/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos
                .Include(v => v.Fabricante)
                .Include(v => v.Concessionaria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        // GET: Veiculo/Create
        [Authorize(Roles = "Gerente")]
        public IActionResult Create()
        {
            ViewData["FabricanteId"] = new SelectList(_context.Fabricantes, "Id", "Nome");
            ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome");
            return View();
        }

        // POST: Veiculo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Create([Bind("Modelo,AnoFabricacao,Placa,TipoVeiculo,FabricanteId,ConcessionariaId,Descricao,Id,PrecoStringView")] Veiculo veiculo)
        {
           veiculo.Preco = Convert.ToDecimal(veiculo.PrecoStringView, new CultureInfo("pt-BR"));
           if (ModelState.IsValid)
            {
                _context.Add(veiculo);
                await _context.SaveChangesAsync();
                TempData["Successo"] = "Veículo criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FabricanteId"] = new SelectList(_context.Fabricantes, "Id", "Nome", veiculo.FabricanteId);
            ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome", veiculo.ConcessionariaId);
            return View(veiculo);
        }

        // GET: Veiculo/Edit/5
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }
            veiculo.PrecoStringView = veiculo.Preco.ToString("N2", new CultureInfo("pt-BR"));
            ViewData["FabricanteId"] = new SelectList(_context.Fabricantes, "Id", "Nome", veiculo.FabricanteId);
            ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome", veiculo.ConcessionariaId);

            return View(veiculo);
        }

        // POST: Veiculo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Edit(int id, [Bind("Modelo,AnoFabricacao,Placa,TipoVeiculo,FabricanteId,ConcessionariaId,Descricao,Id,PrecoStringView")] Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }
            veiculo.Preco = Convert.ToDecimal(veiculo.PrecoStringView, new CultureInfo("pt-BR"));
                    
            if (ModelState.IsValid)
            {
                try
                {
                    var veiculoEdicao = await _context.Veiculos.FindAsync(id);
                    veiculoEdicao.UpdatedAt = DateTime.Now;
                    _context.Veiculos.Update(veiculoEdicao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoExists(veiculo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                    }
                TempData["Successo"] = "Veículo Editado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FabricanteId"] = new SelectList(_context.Fabricantes, "Id", "Nome", veiculo.FabricanteId);
            ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome", veiculo.ConcessionariaId);
            return View(veiculo);
        }

        // GET: Veiculo/Delete/5
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos
                .Include(v => v.Fabricante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        // POST: Veiculo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo != null)
            {
                veiculo.IsDeleted = true;
                _context.Veiculos.Update(veiculo);
            }

            await _context.SaveChangesAsync();
            TempData["Successo"] = "Veículo deletado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        private bool VeiculoExists(int id)
        {
            return _context.Veiculos.Any(e => e.Id == id);
        }
    }
}
