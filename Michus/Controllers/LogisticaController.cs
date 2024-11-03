using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Michus.Service;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Michus.Controllers
{
    [Authorize]
    public class LogisticaController : Controller
    {
        private readonly MenuService _menuService;

        public LogisticaController(MenuService menuService)
        {
            _menuService = menuService;
        }

        // GET: LogisticaController/Productos
        public async Task<ActionResult> Productos()
        {
            await LoadMenuDataAsync(); // Cargar los datos del menú aquí
            return View(); // Renderiza Productos.cshtml
        }

        private async Task LoadMenuDataAsync()
        {
            string roleId = GetCurrentRoleId();
            var menuJson = await _menuService.GetMenusJsonByRoleAsync(roleId);
            ViewData["MenuItems"] = string.IsNullOrEmpty(menuJson) ? "[]" : menuJson;
        }

        private string GetCurrentRoleId()
        {
            var roleIdClaim = User.FindFirst(ClaimTypes.Role);
            return roleIdClaim?.Value ?? string.Empty;
        }
    }
}
