using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConcessionariaManager.Web.Data;
using ConcessionariaManager.Core.Models.AccessDeniedLog;


namespace ConcessionariaManager.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AccessDeniedLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccessDeniedLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? userName, DateTime? date, int page = 1, int pageSize = 10)
        {
            var query = _context.AccessDeniedLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userName))
                query = query.Where(l => l.UserName.Contains(userName));

            if (date.HasValue)
            {
                var start = date.Value.Date;
                var end = start.AddDays(1);
                query = query.Where(l => l.AccessDateTime >= start && l.AccessDateTime < end);
            }

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.AccessDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new AccessDeniedLogViewModel
            {
                Logs = logs,
                UserName = userName,
                Date = date,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(viewModel);
        }

    }
}