using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Michus.Service;
using Michus.DAO;
using System.Threading.Tasks;
using System.Security.Claims;
using Michus.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System;
using System.Data;
using System.Dynamic;
using System.Text.Json;
using Dapper;

namespace Michus.Controllers
{
    [Route("Descuentos")]

    public class DescuentosController : Controller
    {
        private readonly MenuService _menuService;
        private readonly DescuentosDAO _descuentosDAO;
        private readonly string _connectionString;

        public DescuentosController(MenuService menuService, IConfiguration config)
        {
            _connectionString = config.GetConnectionString("cn1");
            _menuService = menuService;
            _descuentosDAO = new DescuentosDAO(_connectionString);
        }

        [HttpGet("ListaDescuentos")]
        public async Task<ActionResult> Listadescuentos(int? FECHA_INICIO = null, int? FECHA_FIN = null, string? TI_SITU = "PAP")
        {
            await LoadMenuDataAsync();

            var aniosInicioDesc = await _descuentosDAO.GetAnioInicioDesc();
            var aniosFinDesc = await _descuentosDAO.GetAnioFinDesc();

            ViewBag.AniosInicioDesc = new SelectList(aniosInicioDesc, "anio", "anio");
            ViewBag.AniosFinDesc = new SelectList(aniosFinDesc, "anio", "anio");

            ViewBag.TiSitu = TI_SITU;
            ViewBag.FechaInicio = FECHA_INICIO;
            ViewBag.FechaFin = FECHA_FIN;

            var descuentos = await _descuentosDAO.GetDescuentosCartilla(FECHA_INICIO, FECHA_FIN, TI_SITU);

            return View(descuentos);
        }

        [HttpGet("ListaDescuentos/Data")]
        public async Task<IActionResult> GetDescuentosData(int? FECHA_INICIO = null, int? FECHA_FIN = null, string? TI_SITU = "PAP")
        {
            var descuentos = await _descuentosDAO.GetDescuentosCartilla(FECHA_INICIO, FECHA_FIN, TI_SITU);

            if (descuentos == null || !descuentos.Any())
            {
                return NotFound(new { message = "No se encontraron descuentos." });
            }

            return Ok(descuentos);
        }

        [HttpGet("DetallesDescuento/{id}")]
        public async Task<IActionResult> GetDetallesDescuento(string id)
        {
            if (id == null)
            {
                return BadRequest(new { error = "ID de descuento inválido." });
            }

            try
            {
                // Crear conexión con la base de datos
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Crear el comando para ejecutar el procedimiento almacenado
                    using (var command = new SqlCommand("SP_VER_DETALLES_DESCUENTO", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir el parámetro para el procedimiento almacenado
                        command.Parameters.AddWithValue("@ID_DESCUENTO", id);

                        // Abrir la conexión
                        await connection.OpenAsync();

                        // Ejecutar el procedimiento almacenado y obtener los resultados
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var descuento = new
                                {
                                    IdDescuento = reader["ID_DESCUENTO"],
                                    IdPromocion = reader["ID_PROMOCION"],
                                    IdEvento = reader["ID_EVENTO"],
                                    FechaInicio = reader["FECHA_INICIO"],
                                    FechaFin = reader["FECHA_FIN"],
                                    PrecioDescuento = reader["PRECIO_DESCUENTO"],
                                    TipoDescuento = reader["TIPO_DESCUENTO"],
                                    Categorias = reader["CATEGORIAS"],
                                    Productos = reader["PRODUCTOS"],
                                    TI_SITU = reader["TI_SITU"]
                                };

                                // Retornar los resultados como JSON
                                return Json(descuento);
                            }

                            return NotFound(new { error = "Descuento no encontrado." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("ActualizarSitu")]
        public IActionResult ActualizarSitu()
        {
            string idDescuento = Request.Form["idDescuento"];
            string situ = Request.Form["situ"];

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("spActualizarSitu", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdDescuento", idDescuento);
                    command.Parameters.AddWithValue("@TI_SITU", situ);

                    try
                    {
                        var result = command.ExecuteNonQuery();

                        if (result == 0)
                        {
                            return BadRequest("No se puede cambiar TI_SITU porque el estado del descuento es 1.");
                        }

                        return Ok($"Estado actualizado a {situ} para el descuento {idDescuento}");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Error al actualizar el estado: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("RegistrarDescuento")]
        public async Task<IActionResult> RegistrarDescuento([FromBody] JsonElement request)
        {
            try
            {
                var fechaInicio = request.GetProperty("FECHA_INICIO").ToString();
                var fechaFin = request.GetProperty("FECHA_FIN").ToString();
                var precioDescuento = request.GetProperty("PRECIO_DESCUENTO").ToString();
                var tipoDescuento = request.GetProperty("TIPO_DESCUENTO").ToString();
                var aplicarCategoria = request.GetProperty("APLICAR_CATEGORIA").ToString();
                var idCategorias = request.GetProperty("ID_CATEGORIA");
                var idArticulos = request.GetProperty("ID_ARTICULOS");

                DateTime fechaInicioDateTime;
                DateTime fechaFinDateTime;

                if (string.IsNullOrWhiteSpace(fechaInicio) || !DateTime.TryParse(fechaInicio, out fechaInicioDateTime))
                {
                    return BadRequest("El formato de la fecha de inicio es inválido.");
                }

                if (string.IsNullOrWhiteSpace(fechaFin) || !DateTime.TryParse(fechaFin, out fechaFinDateTime))
                {
                    return BadRequest("El formato de la fecha de fin es inválido.");
                }

                if (string.IsNullOrWhiteSpace(precioDescuento) || !decimal.TryParse(precioDescuento, out decimal result))
                {
                    return BadRequest("El campo 'PrecioDescuento' es inválido.");
                }

                if (string.IsNullOrEmpty(tipoDescuento))
                {
                    return BadRequest("El campo 'TipoDescuento' es obligatorio.");
                }

                if (aplicarCategoria != null && (aplicarCategoria == "categoria" && idCategorias.ValueKind == JsonValueKind.Null))
                {
                    return BadRequest("Se debe especificar al menos una categoría si el tipo de descuento es 'categoria'.");
                }

                if (aplicarCategoria != null && (aplicarCategoria == "articulo" && idArticulos.ValueKind == JsonValueKind.Null))
                {
                    return BadRequest("Se debe especificar al menos un artículo si el tipo de descuento es 'articulo'.");
                }

                var idArticulosList = new List<string>();
                var idCategoriasList = new List<string>();

                if (idCategorias.ValueKind == JsonValueKind.String)
                {
                    idCategoriasList = idCategorias.GetString()!.Split(',').ToList();
                }

                if (idArticulos.ValueKind == JsonValueKind.String)
                {
                    idArticulosList = idArticulos.GetString()!.Split(',').ToList();
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("SP_CREAR_DESCUENTO", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@FECHA_INICIO", fechaInicioDateTime);
                        command.Parameters.AddWithValue("@FECHA_FIN", fechaFinDateTime);
                        command.Parameters.AddWithValue("@PRECIO_DESCUENTO", precioDescuento);
                        command.Parameters.AddWithValue("@TIPO_DESCUENTO", tipoDescuento);
                        command.Parameters.AddWithValue("@APLICAR_CATEGORIA", aplicarCategoria);
                        command.Parameters.AddWithValue("@ID_CATEGORIA", idCategoriasList.Any() ? string.Join(",", idCategoriasList) : DBNull.Value);
                        command.Parameters.AddWithValue("@ID_ARTICULOS", idArticulosList.Any() ? string.Join(",", idArticulosList) : DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                    }

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("activar-descuento/{idDescuento}")]
        public async Task<IActionResult> ActivarDescuento(string idDescuento)
        {
            if (string.IsNullOrEmpty(idDescuento))
            {
                return BadRequest("El ID de descuento es obligatorio.");
            }

            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@ID_DESCUENTO", idDescuento, DbType.String);

                    var result = await dbConnection.ExecuteAsync("SP_ESTADO_DESCUENTO", parameters, commandType: CommandType.StoredProcedure);

                    return Ok(new { mensaje = "Descuento actualizado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Hubo un error al procesar el descuento", error = ex.Message });
            }
        }

        [HttpGet("ListarProductos")]
        public async Task<IActionResult> ListarProductos()
        {
            var productos = await _descuentosDAO.ListarProductosAsync();
            return Ok(productos);
        }

        [HttpGet("ListarCategorias")]
        public async Task<IActionResult> ListarCategorias()
        {
            var categoria = await _descuentosDAO.ListarCategoriasAsync();
            return Ok(categoria);
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
