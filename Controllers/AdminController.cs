using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Gerenciar usuarios
    public async Task<IActionResult> Usuarios()
    {
        var usuarios = _userManager.Users.ToList();
        var rolesPorUsuario = new Dictionary<string,string>();

        foreach(var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario); 
            var role = roles.FirstOrDefault() ?? "Sem Perfil";
            rolesPorUsuario.Add(usuario.Id, role);
        }
        ViewBag.RolesUsuarios = rolesPorUsuario;
        return View(usuarios);
    }

    // Gerenciar roles
    public async Task<IActionResult> Roles()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    public async Task<IActionResult> TornarAdmin(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario != null)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario,roles);
            await _userManager.AddToRoleAsync(usuario,"Admin");
        }
        return RedirectToAction("Usuarios");
    }



    // TrocarPerfil GET
    public async Task<IActionResult> TrocarPerfil(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToAction("Usuarios");

        var rolesDisponiveis = _roleManager.Roles.ToList();
        var usuario = await _userManager.FindByIdAsync(id);
        ViewBag.RolesDisponiveis = rolesDisponiveis;
        return View(usuario);
    }

    // TrocarPerfil POST
    [HttpPost]
    public async Task<IActionResult> TrocarPerfil(string idUser, string role)
    {
        if (string.IsNullOrEmpty(idUser) || string.IsNullOrEmpty(role))
            return RedirectToAction("Usuarios");

        var usuario = await _userManager.FindByIdAsync(idUser);
        if (usuario != null)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, roles);
            await _userManager.AddToRoleAsync(usuario, role);
        }

        return RedirectToAction("Usuarios");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToAction("Usuarios");

        var usuarioAtual = await _userManager.GetUserAsync(User);
        if (usuarioAtual != null && usuarioAtual.Id == id)
            return RedirectToAction("Usuarios");

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario != null)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            if (roles.Any())
            {
                await _userManager.RemoveFromRolesAsync(usuario, roles);
            }

            await _userManager.DeleteAsync(usuario);
        }

        return RedirectToAction("Usuarios");
    }
}

//POST
