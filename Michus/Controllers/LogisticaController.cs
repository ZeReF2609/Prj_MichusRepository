using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Michus.DAO;
using Michus.Models;
using Michus.Service; // Asegúrate de que la referencia a Michus.Service esté disponible

namespace Michus.Controllers
{
    [Authorize]
    public class LogisticaController : Controller
    {
        private readonly ProductoDAO _productoDAO;
        private readonly MenuService _menuService; // Agregar el servicio de menú

        public LogisticaController(ProductoDAO productoDAO, MenuService menuService)
        {
            _productoDAO = productoDAO;
            _menuService = menuService; // Inyectar el servicio de menú
        }

        // GET: LogisticaController/Productos
        public async Task<IActionResult> Productos()
        {
            var productos = await _productoDAO.ObtenerProductos();
            await LoadMenuDataAsync(); // Cargar datos del menú
            return View("Productos", productos);
        }

        // Modal de creación de producto
        public IActionResult CrearProducto()
        {
            return PartialView("_CrearProducto");
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                await _productoDAO.InsertarProducto(producto);
                return RedirectToAction("Productos");
            }
            return PartialView("_CrearProducto", producto);
        }

        // Modal de edición de producto
        public async Task<IActionResult> EditarProducto(string id)
        {
            var producto = await _productoDAO.ObtenerProductoPorId(id);
            return PartialView("_EditarProducto", producto);
        }

        [HttpPost]
        public async Task<IActionResult> EditarProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                await _productoDAO.ActualizarProducto(producto);
                return RedirectToAction("Productos");
            }
            return PartialView("_EditarProducto", producto);
        }

        // Modal de desactivación de producto
        public async Task<IActionResult> DesactivarProducto(string id)
        {
            var producto = await _productoDAO.ObtenerProductoPorId(id);
            return PartialView("_DesactivarProducto", producto);
        }

        // Cargar datos del menú
        private async Task LoadMenuDataAsync()
        {
            string roleId = GetCurrentRoleId();
            var menuJson = await _menuService.GetMenusJsonByRoleAsync(roleId);
            ViewData["MenuItems"] = string.IsNullOrEmpty(menuJson) ? "[]" : menuJson;
        }

        // Obtener el ID del rol actual del usuario
        private string GetCurrentRoleId()
        {
            var roleIdClaim = User.FindFirst(ClaimTypes.Role);
            return roleIdClaim?.Value ?? string.Empty;
        }
    }
}
