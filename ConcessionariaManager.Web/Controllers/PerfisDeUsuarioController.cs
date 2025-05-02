using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class PerfisDeUsuarioController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PerfisDeUsuarioController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User ID cannot be null or empty.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        ViewBag.UserId = user.Id;
        ViewBag.UserName = user.UserName;

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

        var model = new UserRolesViewModel
        {
            UserRoles = [.. userRoles],
            AllRoles = allRoles.Except(userRoles).ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(string userId, string role)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
        {
            return BadRequest("User ID and role cannot be null or empty.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        await _userManager.AddToRoleAsync(user, role);
        return RedirectToAction(nameof(Index), new { userId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRole(string userId, string role)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
        {
            return BadRequest("User ID and role cannot be null or empty.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        await _userManager.RemoveFromRoleAsync(user, role);
        return RedirectToAction(nameof(Index), new { userId });
    }

    public class UserRolesViewModel
    {
        public List<string?> UserRoles { get; set; } = new List<string?>();
        public List<string?> AllRoles { get; set; } = new List<string?>();
    }
}
