using Microsoft.AspNetCore.Mvc;
using ConcessionariaManager.Core.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using ConcessionariaManager.Core.Interfaces.Repositories;
using ConcessionariaManager.Core.Interfaces.Services;

namespace ConcessionariaManager.Web.Controllers
{
    public class VendaController : Controller
    {
        private readonly IVendaService _service;
        private readonly IVendaRepository _repository;

        public VendaController(IVendaService service, IVendaRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        [Authorize]
        public async Task<IActionResult> Index(string modelo, string cliente, int? page)
        {
            var vendas =  _repository.ObterVendas(modelo, cliente).ToPagedList(page ?? 1, 10);
            return View(vendas);
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> ObterVeiculosPorConcessionaria(int concessionariaId)
        {
            var result = await _service.ObterVeiculosDisponiveisJsonAsync(concessionariaId);
            return Json(result);
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venda = await _repository.ObterVendaPorIdAsync(id.Value);
            return venda == null ? NotFound() : View(venda);
        }

        [Authorize(Roles = "Vendedor")]
        public IActionResult Create()
        {
            ViewData["VeiculoId"] = new SelectList(_repository.ObterVeiculosList().Where(v => !v.Vendido), "Id", "Modelo");
            ViewData["ConcessionariaId"] = new SelectList(_repository.ObterConcessionariasList(), "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Create(Venda venda)
        {
            venda.PrecoVenda = decimal.Parse(venda.PrecoVendaStringView, new CultureInfo("pt-BR"));

            if (!ModelState.IsValid)
                return Create();

            var (sucesso, erro, _) = await _service.CriarVendaAsync(venda);

            if (!sucesso)
            {
                ModelState.AddModelError("", erro ?? "Erro desconhecido.");
                return Create();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Cancelar(int? id)
        {
            if (id == null) return NotFound();

            var venda = await _repository.ObterVendaPorIdAsync(id.Value);
            if (venda == null || venda.Cancelada || DateTime.Now > venda.DataVenda.AddMonths(3))
            {
                TempData["Erro"] = "Venda não encontrada ou prazo expirado.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Sucesso"] = "Venda realizada com sucesso.";
            return View(venda);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> ConfirmarCancelamento(int id, string MotivoDoCancelameto)
        {
            if (string.IsNullOrWhiteSpace(MotivoDoCancelameto))
            {
                ModelState.AddModelError("MotivoDoCancelameto", "O motivo do cancelamento é obrigatório.");
                return await Cancelar(id);
            }

            var (sucesso, erro) = await _service.CancelarVendaAsync(id, MotivoDoCancelameto);
            if (!sucesso) TempData["Erro"] = erro;

            TempData["Sucesso"] = "Cancelamento da venda confirmado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
