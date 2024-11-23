using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Michus.Service;
using Michus.DAO;
using System.Threading.Tasks;
using System.Security.Claims;
using Michus.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using System;
using System.Data;
using System.Dynamic;

namespace Michus.Controllers
{
    [Authorize]
    public class DescuentosController : Controller
    {
        private readonly MenuService _menuService;
        private readonly DescuentosDAO _descuentosDAO;
        private readonly string _connectionString;

        public DescuentosController(MenuService menuService, IConfiguration config)
        {
            _connectionString = config.GetConnectionString("cn1"); // Se asigna la cadena de conexión al campo privado
            _menuService = menuService;
            _descuentosDAO = new DescuentosDAO(_connectionString);
        }




        // Acción para listar descuentos
        public async Task<ActionResult> listadescuentos(int? FECHA_INICIO = null, int? FECHA_FIN = null)
        {
            await LoadMenuDataAsync();

            var aniosInicioDesc = await _descuentosDAO.GetAnioInicioDesc();
            var aniosFinDesc = await _descuentosDAO.GetAnioFinDesc();

            ViewBag.AniosInicioDesc = new SelectList(aniosInicioDesc, "anio", "anio");
            ViewBag.AniosFinDesc = new SelectList(aniosFinDesc, "anio", "anio");

            var descuentos = await _descuentosDAO.GetDescuentosCartilla(FECHA_INICIO, FECHA_FIN);
            return View(descuentos);
        }

       

        [HttpPost("VerDetallesDescuento")]
        public async Task<IActionResult> VerDetallesDescuento([FromBody] string idDescuento)
        {
            if (string.IsNullOrEmpty(idDescuento))
            {
                return BadRequest(new { error = "El ID del descuento es obligatorio." });
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
                        command.Parameters.AddWithValue("@ID_DESCUENTO", idDescuento);

                        // Abrir la conexión
                        await connection.OpenAsync();

                        // Ejecutar el procedimiento almacenado y obtener los resultados
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var resultados = new List<dynamic>();

                            // Leer los resultados
                            while (await reader.ReadAsync())
                            {
                                var row = new ExpandoObject() as IDictionary<string, Object>;

                                // Recorrer las columnas de la fila y almacenarlas en un diccionario
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }

                                // Agregar la fila al resultado
                                resultados.Add(row);
                            }

                            // Retornar los resultados
                            return Ok(resultados);
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







        [HttpGet]
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






        // Cargar datos de menú
        private async Task LoadMenuDataAsync()
        {
            string roleId = GetCurrentRoleId();
            var menuJson = await _menuService.GetMenusJsonByRoleAsync(roleId);
            ViewData["MenuItems"] = string.IsNullOrEmpty(menuJson) ? "[]" : menuJson;
        }

        // Obtener el ID del rol actual
        private string GetCurrentRoleId()
        {
            var roleIdClaim = User.FindFirst(ClaimTypes.Role);
            return roleIdClaim?.Value ?? string.Empty;
        }
    }
}
