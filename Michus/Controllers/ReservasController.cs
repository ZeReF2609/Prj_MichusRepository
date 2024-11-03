using Michus.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Michus.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly MenuService _menuService;

        public ReservasController(MenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        [Route("Reservas/listar-reserva")] 
        public async Task<ActionResult> ListarReserva()
        {
            await LoadMenuDataAsync();
            return View();
        }

        [HttpGet]
        [Route("Reservas/listar-clientes")]
        public async Task<ActionResult> ListarClientes()
        {
            await LoadMenuDataAsync();
            return View();
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
