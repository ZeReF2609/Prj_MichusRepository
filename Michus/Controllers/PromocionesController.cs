
using Michus.DAO;
using Michus.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NewtonsoftJson = Newtonsoft.Json;

namespace Michus.Controllers
{
    public class PromocionesController : Controller
    {
        private readonly MenuService _menuService;
        private readonly string _cnx;

        private readonly CorreoHelper _correoHelper;

        public PromocionesController(IConfiguration configuration,MenuService menuService, CorreoHelper correoHelper)
        {
            _cnx = configuration.GetConnectionString("cn1")!;
            _menuService = menuService;
            _correoHelper = correoHelper;
        }

        // PROBANDO SI FUNCIONA ENVIAR ARCHIVOS PUNTUALES


        [HttpGet]
        public async Task<IActionResult> EnviarTokenPorPromocion([FromQuery] string destinatario)
        {
            if (string.IsNullOrEmpty(destinatario))
            {
                return BadRequest("El destinatario no puede estar vacío.");
            }

            try
            {
                await _correoHelper.EnviarCorreoConTokenAsync(destinatario);
                return Ok("Correo enviado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
                return StatusCode(500, "Hubo un problema al enviar el correo: " + ex.Message);
            }
        }


        [HttpPost]
        public IActionResult ValidarToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token) || request.IdPromocion == 0)
            {
                return Json(new { success = false, message = "Token o ID de promoción no proporcionado." });
            }

            bool isValid = _correoHelper.ValidarToken(request.Token);

            if (isValid)
            {
                ActualizarEstadoPromo(request.IdPromocion);
                return Json(new { success = true, message = "Token válido. Promoción activada." });
            }
            else
            {
                return Json(new { success = false, message = "Token inválido o expirado." });
            }
        }


        // Actualizar el estado de la promoción
        public void ActualizarEstadoPromo(int idPromocion)
        {
            using (SqlConnection cnn = new SqlConnection(_cnx))
            {
                try
                {
                    cnn.Open();

                    using (SqlCommand cmm = new SqlCommand("SP_ESTADO_PROMOCION", cnn))
                    {
                        cmm.CommandType = CommandType.StoredProcedure;

                        cmm.Parameters.Add(new SqlParameter("@ID_PROMOCION", SqlDbType.Int));
                        cmm.Parameters["@ID_PROMOCION"].Value = idPromocion;

                        cmm.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error: {ex.Message}");
                }
            }
        }


        public class TokenRequest
        {
            public string Token { get; set; }
            public int IdPromocion { get; set; }
        }


        // GET: PromocionesController
        public async Task<ActionResult> ListarPromociones(int? idPromocion = 1)
        {
            SetNoCacheHeaders();

            var promociones = new List<dynamic>();
            var promocionesFiltro = new List<dynamic>();
            var productos = new List<dynamic>();
            var detallePromociones = new List<dynamic>();

            using (SqlConnection connection = new SqlConnection(_cnx))
            {
                await connection.OpenAsync();

                using (SqlCommand cmm = new SqlCommand("SP_LISTAR_PROMOCIONES", connection))
                {
                    cmm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader rd = await cmm.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            promociones.Add(new
                            {
                                IdPromociones = rd["ID_PROMOCION"].ToString(),
                                NomPromo = rd["NOMBRE_PROMOCION"],
                                TipoPromocion = rd["TIPO_PROMOCION"],
                                Descuento = rd["DESCUENTO"],
                                Descripcion = rd["DESCRIPCION"],
                                FechaInicio = rd["FECHA_INICIO"],
                                FechaFin = rd["FECHA_FIN"],
                                Estado = rd["ESTADO"]
                            });
                        }
                    }
                }

                using (SqlCommand cmm = new SqlCommand("SP_LISTAR_PROMOCIONES", connection))
                {
                    cmm.CommandType = CommandType.StoredProcedure;

                    cmm.Parameters.Add(new SqlParameter("@FiltrarActivas", SqlDbType.Bit)
                    {
                        Value = 1 
                    });

                    using (SqlDataReader rd = await cmm.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            promocionesFiltro.Add(new
                            {
                                IdPromociones = rd["ID_PROMOCION"].ToString(),
                                NomPromo = rd["NOMBRE_PROMOCION"],
                                TipoPromocion = rd["TIPO_PROMOCION"],
                                Descuento = rd["DESCUENTO"],
                                Descripcion = rd["DESCRIPCION"],
                                FechaInicio = rd["FECHA_INICIO"],
                                FechaFin = rd["FECHA_FIN"],
                                Estado = rd["ESTADO"]
                            });
                        }
                    }
                }


                ViewBag.PromocionesList = new SelectList(promocionesFiltro, "IdPromociones", "NomPromo", idPromocion);


                using (SqlCommand cmm = new SqlCommand("SP_LISTAR_PRODUCTOS_PROMOS", connection))
                {
                    cmm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader rd = await cmm.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            productos.Add(new
                            {
                                IdProductos = rd["ID_PRODUCTO"].ToString(),
                                Producto = rd["PROD_NOM"],
                                Descripcion = rd["DESCRIPCION"],
                                Categoria = rd["CATEGORIA"],
                                FechaDisponible = rd["PROD_FCHCMRL"].ToString(),
                                Precio = rd["PRECIO"].ToString(),
                                Estado = rd["ESTADO"]

                            });
                        }
                    }
                }

                using (SqlCommand cmmD = new SqlCommand("SP_LISTAR_DETALLE_PROMOCIONES", connection))
                {
                    cmmD.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader rd = await cmmD.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            detallePromociones.Add(new
                            {
                                IdDetalle = rd["ID_REF"].ToString(),
                                Promocion = rd["NOMBRE_PROMOCION"],
                                Producto = rd["PROD_NOM"],
                                CantAplicable = rd["CANTIDAD_APLICABLE"],
                                TipoAplicacion = rd["TIPO_APLICACION"]
                            });
                        }
                    }
                }
            }


            ViewBag.Productos = productos;
            ViewBag.Promociones = promociones;
            ViewBag.PromocionesFiltradas = promocionesFiltro;
            ViewBag.DetallePromocion = detallePromociones;

            await LoadMenuDataAsync();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarDetallePromo(int idPromocion, string selectedProducts, int cantidadAplicable)
        {
            try
            {

                using (var connection = new SqlConnection(_cnx))
                {
                    // Abrimos la conexión
                    await connection.OpenAsync();

                    // Iniciar la transacción
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            // Creamos el comando para ejecutar el procedimiento almacenado
                            using (var command = new SqlCommand("SP_CREAR_DETALLE_PROMOCION", connection, (SqlTransaction)transaction))
                            {
                                command.CommandType = System.Data.CommandType.StoredProcedure;

                                // Añadimos los parámetros necesarios para el procedimiento almacenado
                                command.Parameters.Add(new SqlParameter("@ID_PROMOCION", idPromocion));
                                command.Parameters.Add(new SqlParameter("@ID_PRODUCTO", selectedProducts)); 
                                command.Parameters.Add(new SqlParameter("@CANTIDAD_APLICABLE", cantidadAplicable));

                                // Ejecutamos el procedimiento y esperamos su resultado
                                var result = await command.ExecuteNonQueryAsync();

                                // Verificamos si la ejecución fue exitosa
                                
                            }

                            // Si todo fue exitoso, confirmamos la transacción
                            await transaction.CommitAsync();
                            return RedirectToAction("ListarPromociones");
                        }
                        catch (Exception)
                        {
                            TempData["ErrorMensaje"] = "Error al aplicar el descuento. El descuento no debe superar el precio del producto.";
                            return View("ListarPromociones");
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Capturamos cualquier error inesperado
                return View("Error inesperado, intentalo más tarde");
            }
        }







        [HttpPost]
        public async Task<ActionResult> CrearPromocion(string nombrePromocion, byte tipoPromocion, decimal descuento, string descripcion, DateTime fechaInicio, DateTime fechaFin, byte estado = 0)
        {
            if (string.IsNullOrEmpty(nombrePromocion) || descuento <= 0 || string.IsNullOrEmpty(descripcion) || fechaInicio == default || fechaFin == default)
            {
                ViewBag.ErrorMessage = "Todos los campos son requeridos.";
                return RedirectToAction("ListarPromociones");
            }

            if (tipoPromocion != 0 && tipoPromocion != 1)
            {
                ViewBag.ErrorMessage = "Tipo de promoción inválido.";
                return RedirectToAction("ListarPromociones");
            }

            using (SqlConnection connection = new SqlConnection(_cnx))
            {
                try
                {
                    // Abrir conexión de forma asíncrona
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SP_CREAR_PROMOCION", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@NOMBRE_PROMOCION", nombrePromocion);
                        command.Parameters.AddWithValue("@TIPO_PROMOCION", tipoPromocion);
                        command.Parameters.AddWithValue("@DESCUENTO", descuento);
                        command.Parameters.AddWithValue("@DESCRIPCION", descripcion);
                        command.Parameters.AddWithValue("@FECHA_INICIO", fechaInicio);
                        command.Parameters.AddWithValue("@FECHA_FIN", fechaFin);
                        command.Parameters.AddWithValue("@ESTADO", estado);

                        // Ejecutar el procedimiento almacenado de forma asíncrona
                        await command.ExecuteNonQueryAsync();

                        return RedirectToAction("ListarPromociones"); 
                    }
                }
                catch (Exception)
                {
                    // Manejo de errores
                    TempData["ErrorMessage"] = "Ocurrió un error, por favor intenta de nuevo más tarde.";
                    return RedirectToAction("ListarPromociones");
                }
            }
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
            catch (NewtonsoftJson.JsonException)
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





        // GET: PromocionesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PromocionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PromocionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PromocionesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PromocionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PromocionesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PromocionesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
