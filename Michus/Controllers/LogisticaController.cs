using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Michus.Models;
using Michus.DAO;
using Michus.Service;
using System.Security.Claims;


namespace Michus.Controllers
{
    public class LogisticaController : Controller
    {
        private readonly ProductoDao _productoService;
        private readonly MenuService _menuService;
        private const int PageSize = 5; 

        public LogisticaController(ProductoDao productoService, MenuService menuService)
        {
            _productoService = productoService;
            _menuService = menuService;
        }

        // Método para listar productos con paginación
        [HttpGet("/Logistica/productos")]
        public async Task<IActionResult> Producto(int page = 1)
        {
            var productos = await _productoService.ObtenerProductos();
            await LoadMenuDataAsync();
            var paginatedList = productos
                .OrderBy(p => p.IdProducto)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)System.Math.Ceiling((double)productos.Count / PageSize);
            
            return View(paginatedList);
        }

        // Métodos para insertar, editar y cambiar estado (sin cambios mayores)
        [HttpPost]
        public async Task<IActionResult> InsertarProductos(Producto producto)
        {
            if (ModelState.IsValid)
            {
                await _productoService.InsertarProductos(producto);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ObtenerProductoPorId(string id)
        {
            var producto = await _productoService.ObtenerProductoPorId(id);
            if (producto == null)
            {
                return NotFound();
            }
            return Json(producto);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                await _productoService.ActualizarProducto(producto);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DesactivarProducto(string id)
        {
            await _productoService.DesactivarProducto(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ActivarProducto(string id)
        {
            await _productoService.ActivarProducto(id);
            return RedirectToAction("Index");
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