using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class PerfilController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public PerfilController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string roleName)
    {
        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (result.Succeeded)
            return RedirectToAction(nameof(Index));

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View();
    }

    public async Task<IActionResult> Delete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role != null)
            await _roleManager.DeleteAsync(role);

        return RedirectToAction(nameof(Index));
    }
}
