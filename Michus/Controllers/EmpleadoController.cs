using Michus.DAO;
using Michus.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Michus.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly EmpleadoDAO _empleadoDao;
        private readonly MenuService _menuService;

        public EmpleadoController(EmpleadoDAO empleadoDao, MenuService menuService)
        {
            _empleadoDao = empleadoDao;
            _menuService = menuService;
        }

        [HttpGet("Usuarios/lista-empleado")]
        [Authorize]
        public async Task<IActionResult> ListaEmpleados()
        {
            try
            {
                var empleados = _empleadoDao.ObtenerTodosEmpleado();

                await LoadMenuDataAsync();

                if (empleados == null || empleados.Count == 0)
                {
                    return NotFound("No se encontraron empleados.");
                }

                return View(empleados);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos de los empleados: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
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
