using Michus.DAO;
using Michus.Models;
using Michus.Service;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        ViewData["TotalPages"] = (int)Math.Ceiling((double)productos.Count / PageSize);

        return View(paginatedList);
    }

    [HttpPost("/Logistica/InsertarProductos")]
    public async Task<IActionResult> InsertarProductos([FromBody] Producto producto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (string.IsNullOrWhiteSpace(producto.ProdNom))
                return BadRequest("El nombre del producto es requerido");
            if (string.IsNullOrWhiteSpace(producto.ProdNomweb))
                return BadRequest("El nombre web del producto es requerido");
            if (string.IsNullOrWhiteSpace(producto.Descripcion))
                return BadRequest("La descripción del producto es requerida");
            if (string.IsNullOrWhiteSpace(producto.IdCategoria))
                return BadRequest("La categoría del producto es requerida");
            if (producto.Precio <= 0)
                return BadRequest("El precio debe ser mayor a 0");

            var newProductId = await _productoService.InsertarProductos(producto);
            return Ok(new { success = true, message = "Producto insertado correctamente", id = newProductId });
        }
        catch (Exception ex)
        {
            // Log the exception here
            return StatusCode(500, new { success = false, message = $"Error al insertar producto: {ex.Message}" });
        }
    }

    [HttpGet("/Logistica/ObtenerProductoPorId")]
    public async Task<IActionResult> ObtenerProductoPorId(string id)
    {
        var producto = await _productoService.ObtenerProductoPorId(id);
        if (producto == null)
        {
            return NotFound();
        }
        return Ok(producto);
    }

    [HttpPost("/Logistica/ActualizarProducto")]
    public async Task<IActionResult> ActualizarProducto([FromBody] Producto producto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (string.IsNullOrWhiteSpace(producto.ProdNom))
                return BadRequest("El nombre del producto es requerido");
            if (string.IsNullOrWhiteSpace(producto.ProdNomweb))
                return BadRequest("El nombre web del producto es requerido");
            if (string.IsNullOrWhiteSpace(producto.Descripcion))
                return BadRequest("La descripción del producto es requerida");
            if (string.IsNullOrWhiteSpace(producto.IdCategoria))
                return BadRequest("La categoría del producto es requerida");
            if (producto.Precio <= 0)
                return BadRequest("El precio debe ser mayor a 0");

            await _productoService.ActualizarProducto(producto);
            return Ok(new { success = true, message = "Producto actualizado correctamente" });
        }
        catch (Exception ex)
        {
            // Log the exception here
            return StatusCode(500, new { success = false, message = $"Error al actualizar producto: {ex.Message}" });
        }
    }

    [HttpPost("/Logistica/DesactivarProducto")]
    public async Task<IActionResult> DesactivarProducto(string id)
    {
        try
        {
            await _productoService.DesactivarProducto(id);
            return Ok(new { success = true, message = "Producto desactivado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error al desactivar producto: {ex.Message}" });
        }
    }

    [HttpPost("/Logistica/ActivarProducto")]
    public async Task<IActionResult> ActivarProducto(string id)
    {
        try
        {
            await _productoService.ActivarProducto(id);
            return Ok(new { success = true, message = "Producto activado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error al activar producto: {ex.Message}" });
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

