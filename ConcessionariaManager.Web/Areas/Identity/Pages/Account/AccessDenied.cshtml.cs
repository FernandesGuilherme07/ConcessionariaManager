using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ConcessionariaManager.Web.Data;
using ConcessionariaManager.Core.Models.AccessDeniedLog;

public class AccessDeniedModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AccessDeniedModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var attemptedUrl = HttpContext.Request.Headers["Referer"].ToString();
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            var log = new AccessDeniedLog
            {
                UserId = userId,
                UserName = user?.UserName ?? "Desconhecido",
                AttemptedUrl = attemptedUrl ?? "Desconhecido",
                Location = userIp,
                AccessDateTime = DateTime.UtcNow
            };

            _context.AccessDeniedLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
