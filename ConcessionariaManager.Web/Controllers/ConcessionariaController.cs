using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Web.Data;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ConcessionariaManager.Web.Controllers
{
    public class ConcessionariaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConcessionariaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index(string sortOrder, string nome, string localizacao, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NomeSortParam"] = String.IsNullOrEmpty(sortOrder) ? "nome_desc" : "";
            ViewData["TelefoneSortParam"] = sortOrder == "telefone" ? "telefone_desc" : "telefone";
            ViewData["EmailSortParam"] = sortOrder == "email" ? "email_desc" : "email";
            ViewData["CreatedAtSortParam"] = sortOrder == "createdat" ? "createdat_desc" : "createdat";
            ViewData["UpdatedAtSortParam"] = sortOrder == "updatedat" ? "updatedat_desc" : "updatedat";

            var concessionarias = _context.Concessionarias.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                concessionarias = concessionarias.Where(c => c.Nome != null && c.Nome.Contains(nome));

            if (!string.IsNullOrEmpty(localizacao))
                concessionarias = concessionarias.Where(c => c.Endereco != null && c.Endereco.EnderecoCompleto.Contains(localizacao));

            concessionarias = sortOrder switch
            {
                "nome_desc" => concessionarias.OrderByDescending(c => c.Nome),
                "telefone" => concessionarias.OrderBy(c => c.Telefone),
                "telefone_desc" => concessionarias.OrderByDescending(c => c.Telefone),
                "email" => concessionarias.OrderBy(c => c.Email),
                "email_desc" => concessionarias.OrderByDescending(c => c.Email),
                "createdat" => concessionarias.OrderBy(c => c.CreatedAt),
                "createdat_desc" => concessionarias.OrderByDescending(c => c.CreatedAt),
                "updatedat" => concessionarias.OrderBy(c => c.UpdatedAt),
                "updatedat_desc" => concessionarias.OrderByDescending(c => c.UpdatedAt),
                _ => concessionarias.OrderByDescending(c => c.CreatedAt),
            };

            int pageSize = 5;
            int pageNumber = page ?? 1;

            return View(concessionarias.ToPagedList(pageNumber, pageSize));
        }


        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concessionaria = await _context.Concessionarias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (concessionaria == null)
            {
                return NotFound();
            }

            return View(concessionaria);
        }

        // GET: Concessionaria/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Concessionaria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Nome,Telefone,Email,CapacidadeMaximaDeVeiculos,Id,Endereco")] Concessionaria concessionaria)
        {
            if (ModelState.IsValid)
            {
                if (_context.Concessionarias.Any(c => c.Nome == concessionaria.Nome))
                {
                    ModelState.AddModelError("Nome", "O nome da concessionária já está em uso.");
                    return View(concessionaria);
                }

                if (_context.Concessionarias.Any(c => c.Email == concessionaria.Email))
                {
                    ModelState.AddModelError("Email", "O e-mail já está em uso.");
                    return View(concessionaria);
                }

                if (_context.Concessionarias.Any(c => c.Telefone == concessionaria.Telefone))
                {
                    ModelState.AddModelError("Telefone", "O Número de telefone já está em uso.");
                    return View(concessionaria);
                }

                _context.Add(concessionaria);
                await _context.SaveChangesAsync();
                TempData["Successo"] = "Concessionária criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(concessionaria);
        }

        // POST: Concessionaria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Nome,Telefone,Email,CapacidadeMaximaDeVeiculos,Id,Endereco")] Concessionaria concessionaria)
        {
            if (id != concessionaria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Concessionarias.Any(c => c.Nome == concessionaria.Nome && c.Id != id))
                    {
                        ModelState.AddModelError("Nome", "O nome da concessionária já está em uso.");
                        return View(concessionaria);
                    }

                    if (_context.Concessionarias.Any(c => c.Email == concessionaria.Email && c.Id != id))
                    {
                        ModelState.AddModelError("Email", "O e-mail já está em uso.");
                        return View(concessionaria);
                    }

                    if (_context.Concessionarias.Any(c => c.Telefone == concessionaria.Telefone && c.Id != id))
                    {
                        ModelState.AddModelError("Telefone", "O Número de telefone já está em uso.");
                        return View(concessionaria);
                    }

                    concessionaria.UpdatedAt = DateTime.UtcNow;
                    _context.Update(concessionaria);
                    await _context.SaveChangesAsync();
                    TempData["Successo"] = "Concessionária atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConcessionariaExists(concessionaria.Id))
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
            return View(concessionaria);
        }


        // GET: Concessionaria/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concessionaria = await _context.Concessionarias.FindAsync(id);
            if (concessionaria == null)
            {
                return NotFound();
            }
            return View(concessionaria);
        }

        // GET: Concessionaria/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concessionaria = await _context.Concessionarias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (concessionaria == null)
            {
                return NotFound();
            }

            return View(concessionaria);
        }

        // POST: Concessionaria/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var concessionaria = await _context.Concessionarias.FindAsync(id);
            if (concessionaria != null)
            {
                concessionaria.IsDeleted = true;
                _context.Concessionarias.Update(concessionaria);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConcessionariaExists(int id)
        {
            return _context.Concessionarias.Any(e => e.Id == id);
        }
    }
}
