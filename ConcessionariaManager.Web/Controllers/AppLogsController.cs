namespace ConcessionariaManager.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using global::ConcessionariaManager.Core.Models.AppLog.ConcessionariaManager.Core.Models.AppLog;
    using global::ConcessionariaManager.Web.Data;

    namespace ConcessionariaManager.Web.Controllers
    {
        [Authorize(Roles = "Administrador")]
        public class AppLogsController : Controller
        {
            private readonly ApplicationDbContext _context;

            public AppLogsController(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IActionResult> Index(string? user, DateTime? date, int page = 1, int pageSize = 10)
            {
                var query = _context.Logs
                    .Include(l => l.LogDetails)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(user))
                    query = query.Where(l => l.User.Contains(user));


                if (date.HasValue)
                {
                    var start = date.Value.Date;
                    var end = start.AddDays(1);
                    query = query.Where(l => l.Date >= start && l.Date < end);
                }

                var totalCount = await query.CountAsync();

                var logs = await query
                    .OrderByDescending(l => l.Date)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new LogViewModel
                {
                    Logs = logs,
                    User = user,
                    Date = date,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return View(viewModel);
            }
        }
    }

}
