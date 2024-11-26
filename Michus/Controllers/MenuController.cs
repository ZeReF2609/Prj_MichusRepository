using Michus.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Michus.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly MenuService _menuService;
        
        public MenuController(MenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            // Configura encabezados para evitar el almacenamiento en caché
            SetNoCacheHeaders();

            // Carga los datos del menú según el rol del usuario actual
            await LoadMenuDataAsync();
            return View();
        }

        private string GetCurrentRoleId()
        {
            // Obtiene el ID del rol del usuario, si está disponible
            var roleIdClaim = User.FindFirst(ClaimTypes.Role);
            return roleIdClaim?.Value ?? string.Empty;
        }

        private async Task LoadMenuDataAsync()
        {
            string roleId = GetCurrentRoleId();

            // Si el rol no está disponible, establece los elementos de menú como vacíos para evitar errores
            if (string.IsNullOrEmpty(roleId))
            {
                ViewData["MenuItems"] = "[]";
                return;
            }

            var menuJson = await _menuService.GetMenusJsonByRoleAsync(roleId);

            // Verifica si el JSON es válido, en caso contrario, define un menú vacío
            if (string.IsNullOrEmpty(menuJson) || !IsValidJson(menuJson))
            {
                menuJson = "[]";
            }

            ViewData["MenuItems"] = menuJson;
        }

        private bool IsValidJson(string json)
        {
            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private void SetNoCacheHeaders()
        {
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, post-check=0, pre-check=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }
    }
}

//ACTUALIZADO A LA FECHA 26/11/2024