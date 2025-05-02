using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConcessionariaManager.Core.Models;
using ConcessionariaManager.Web.Data;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace ConcessionariaManager.Web.Controllers
{
    public class VendaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Venda/Index
        [Authorize]
        public IActionResult Index(string searchString, int? page)
        {
            var vendas = from v in _context.Vendas
                         select v;

            if (!string.IsNullOrEmpty(searchString))
            {
                vendas = vendas.Where(v => v.NomeCliente.Contains(searchString));
            }

            int pageSize = 10;

            var pagedVendas = vendas
                .OrderBy(v => v.DataVenda)
                .Include(v => v.Veiculo)
                .Include(v => v.Concessionaria)
                .AsNoTracking()
                .ToPagedList(page ?? 1, pageSize);

            return View(pagedVendas);
        }
        [HttpGet]
        [Authorize]
        public JsonResult ObterVeiculosPorConcessionaria(int concessionariaId)
        {
            var veiculos = _context.Veiculos
                .Where(v => v.ConcessionariaId == concessionariaId && v.Vendido == false)
                .Select(v => new { v.Id, v.Modelo })
                .ToList();

            return Json(veiculos);
        }

        // GET: Venda/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(venda => venda.Veiculo)
                .Include(venda => venda.Concessionaria)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // GET: Venda/Create
        [Authorize(Roles = "Vendedor")]
        public IActionResult Create()
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos.Where(v => !v.Vendido), "Id", "Modelo");
            ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome");
            return View();
        }

        // POST: Venda/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Create([Bind("ConcessionariaId,VeiculoId,NomeCliente,CPFCliente,TelefoneCliente,DataVenda,PrecoVendaStringView")] Venda venda)
        {
            venda.Cancelada = false;
            venda.ProtocoloVenda = Guid.NewGuid().ToString();
            venda.PrecoVenda = Convert.ToDecimal(venda.PrecoVendaStringView, new CultureInfo("pt-BR"));

            if (!ModelState.IsValid)
            {
                ViewData["VeiculoId"] = new SelectList(_context.Veiculos.Where(v => !v.Vendido), "Id", "Modelo", venda.VeiculoId);
                ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome", venda.ConcessionariaId);
                return View(venda);
            }

            var veiculo = await _context.Veiculos.FirstOrDefaultAsync(v => v.Id == venda.VeiculoId);
            if (veiculo == null)
            {
                ModelState.AddModelError("", "Veículo não encontrado.");
                ViewData["VeiculoId"] = new SelectList(_context.Veiculos.Where( v =>  !v.Vendido), "Id", "Modelo", venda.VeiculoId);
                ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome", venda.ConcessionariaId);
                return View(venda);
            }

            if (venda.PrecoVenda > veiculo.Preco)
            {
                ModelState.AddModelError("PrecoVendaStringView", $"O preço da venda não pode ser maior que o preço do veículo (R$ {veiculo.Preco:N2}).");
                ViewData["VeiculoId"] = new SelectList(_context.Veiculos.Where(v => !v.Vendido), "Id", "Modelo", venda.VeiculoId);
                ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome", venda.ConcessionariaId);
                return View(venda);
            }

            if (veiculo.Vendido)
            {
                ModelState.AddModelError("PrecoVendaStringView", $"O veículo selecionado já foi vendido");
                ViewData["VeiculoId"] = new SelectList(_context.Veiculos.Where(v => !v.Vendido), "Id", "Modelo", venda.VeiculoId);
                ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome", venda.ConcessionariaId);
                return View(venda);
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Vendas.Add(venda);
                veiculo.Vendido = true;
                _context.Veiculos.Update(veiculo);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Erro ao realizar a venda.");
                ViewData["VeiculoId"] = new SelectList(_context.Veiculos.Where(v => !v.Vendido), "Id", "Modelo", venda.VeiculoId);
                ViewData["ConcessionariaId"] = new SelectList(_context.Concessionarias, "Id", "Nome", venda.ConcessionariaId);
                return View(venda);
            }
        }
        // GET: Venda/Cancelar/5
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Cancelar(int? id)
        {
            if (id == null) return NotFound();

            var venda = await _context.Vendas
                .Include(v => v.Veiculo)
                .Include(v => v.Concessionaria)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null || venda.Cancelada) return NotFound();

            var prazoLimite = venda.DataVenda.AddMonths(3);
            if (DateTime.Now > prazoLimite)
            {
                TempData["Erro"] = "A venda não pode ser cancelada após 3 meses da data da venda.";
                return RedirectToAction(nameof(Index));
            }

            return View(venda);
        }

        // POST: Venda/Cancelar/5
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        [HttpPost]
        public async Task<IActionResult> ConfirmarCancelamento(int id, string MotivoDoCancelameto)
        {
            if (string.IsNullOrWhiteSpace(MotivoDoCancelameto))
            {
                ModelState.AddModelError("MotivoDoCancelameto", "O motivo do cancelamento é obrigatório.");
                return await Cancelar(id);
            }

            var venda = await _context.Vendas
                .Include(v => v.Veiculo)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null || venda.Cancelada) return NotFound();

            var prazoLimite = venda.DataVenda.AddMonths(3);
            if (DateTime.Now > prazoLimite)
            {
                TempData["Erro"] = "A venda não pode ser cancelada após 3 meses da data da venda.";
                return RedirectToAction(nameof(Index));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                venda.Cancelada = true;
                venda.MotivoDoCancelameto = MotivoDoCancelameto;
                venda.Veiculo!.Vendido = false;

                _context.Vendas.Update(venda);
                _context.Veiculos.Update(venda.Veiculo);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Erro"] = "Erro ao cancelar a venda.";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
