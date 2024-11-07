using Michus.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Michus.Controllers
{
    public class PromocionesController : Controller
    {

        private readonly MenuService _menuService;

        private readonly string _cnx;

        public PromocionesController(IConfiguration configuration,MenuService menuService)
        {
            _cnx = configuration.GetConnectionString("cn1")!;
            _menuService = menuService;
        }

        // GET: PromocionesController
        public async Task<ActionResult> ListarPromociones(int? idPromocion = 1)
        {
            SetNoCacheHeaders();

            var promociones = new List<dynamic>();
            var detallePromociones = new List<dynamic>();

            using (SqlConnection connection = new SqlConnection(_cnx))
            {
                await connection.OpenAsync();

                using(SqlCommand cmm = new SqlCommand("SP_LISTAR_PROMOCIONES", connection))
                {
                    cmm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader rd = await cmm.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            promociones.Add(new
                            {
                                IdPromociones = rd["ID_PROMOCION"],
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

                ViewBag.PromocionesList = new SelectList(promociones, "IdPromociones", "NomPromo", idPromocion);

                using (SqlCommand cmmD = new SqlCommand("SP_LISTAR_DETALLE_PROMOCIONES", connection))
                {
                    cmmD.CommandType = CommandType.StoredProcedure;
                    cmmD.Parameters.Add(new SqlParameter("@ID_PROMOCION", idPromocion));

                    using (SqlDataReader rd = await cmmD.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            detallePromociones.Add(new
                            {
                                IdDetaPromocion = rd["ID_DETALLE_PROMOCION"],
                                IdPromocion = rd["ID_PROMOCION"],
                                IdProducto = rd["ID_PRODUCTO"],
                                CantAplicable = rd["CANTIDAD_APLICABLE"],
                                TipoAplicacion = rd["TIPO_APLICACION"]
                            });
                        }
                    }
                }
            }

            
            
            ViewBag.Promociones = promociones;
            ViewBag.DetallePromocion = detallePromociones;

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
