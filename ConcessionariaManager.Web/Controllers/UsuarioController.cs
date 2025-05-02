using ConcessionariaManager.Web.Areas.Identity.Pages.Account;
using ConcessionariaManager.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using X.PagedList.Extensions;

[Authorize(Roles = "Administrador")]
public class UsuarioController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<IdentityUser> _signInManager;

    public UsuarioController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _signInManager = signInManager;
    }

    public IActionResult Index(string email, int? page)
    {
        ViewData["Email"] = email;

        var users = _userManager.Users.AsQueryable();

        // Filtro
        if (!string.IsNullOrEmpty(email))
        {
            users = users.Where(u => u.Email != null && u.Email.Contains(email));
        }

        int pageSize = 5;
        int pageNumber = page ?? 1;

        return View(users.ToPagedList(pageNumber, pageSize));
    }
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var allRoles = _roleManager.Roles.ToList();

        var model = new UserEditViewModel
        {
            UserId = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            CurrentRoles = userRoles,
            AllRoles = allRoles
        };

        return View(model);
    }
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (!ModelState.IsValid)
            {
                model.AllRoles = _roleManager.Roles.ToList();
                model.CurrentRoles = await _userManager.GetRolesAsync(user);
                return View(model);
            }
            if (user == null)
            {
                return NotFound();
            }

            var existingUserByName = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserByName != null && existingUserByName.Id != user.Id)
            {
                ModelState.AddModelError("UserName", "Nome de usuário já está em uso.");
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null && existingUserByEmail.Id != user.Id)
            {
                ModelState.AddModelError("Email", "Email já está em uso.");
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                model.AllRoles = _roleManager.Roles.ToList();
                model.CurrentRoles = await _userManager.GetRolesAsync(user);
                return View(model);
            }

            // Atualiza as roles como antes
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = model.SelectedRoles!.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(model.SelectedRoles!);

            foreach (var role in rolesToAdd)
                await _userManager.AddToRoleAsync(user, role);

            foreach (var role in rolesToRemove)
                await _userManager.RemoveFromRoleAsync(user, role);

            // Atualizando as permissões da sessão
            await _signInManager.RefreshSignInAsync(user);

            await transaction.CommitAsync();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError("", "Erro ao atualizar usuário. Detalhes: " + ex.Message);
            return View(model);
        }
    }

    // Deletar Usuário
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.UserName == User.Identity.Name)
        {
            ModelState.AddModelError(string.Empty, "Você não pode excluir a sua própria conta.");
            return View(user);
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(user);
    }

    public async Task<IActionResult> Create()
    {
        var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        var model = new UserCreateViewModel
        {
            AvailableRoles = roles
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync();
            return View(model);
        }

        if (await _userManager.FindByNameAsync(model.UserName!) != null)
        {
            ModelState.AddModelError("UserName", "Nome de usuário já está em uso.");
        }

        if (await _userManager.FindByEmailAsync(model.Input.Email!) != null)
        {
            ModelState.AddModelError("Input.Email", "Email já está em uso.");
        }

        var user = new IdentityUser
        {
            UserName = model.UserName,
            Email = model.Input.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Input.Password);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
            }

            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        model.AvailableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);

        ViewData["Roles"] = roles;

        return View(user);
    }

    public class UserEditViewModel
    {
        public required string UserId { get; set; }
        [Display(Name = "Nome do Usuário")]
        [MaxLength(100)]
        [Required(ErrorMessage = "O campo Nome do usuário é obrigatório.")]
        public required string UserName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
        [Required(ErrorMessage = "O campo Email usuário é obrigatório.")]
        public required string Email { get; set; }
        public IList<string>? CurrentRoles { get; set; }
        public IList<IdentityRole>? AllRoles { get; set; }
        public IEnumerable<string>? SelectedRoles { get; set; }
    }
    public class UserCreateViewModel
    {
        public RegisterModel.InputModel Input { get; set; } = new();

        [Display(Name = "Nome do Usuário")]
        [MaxLength(100)]
        [Required(ErrorMessage = "O campo Nome do usuário é obrigatório.")]
        public string? UserName { get; set; }
        public string? SelectedRole { get; set; }

        [Display(Name = "Nome")]
        [MaxLength(100)]
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public List<string?> AvailableRoles { get; set; } = new();
    }

}
