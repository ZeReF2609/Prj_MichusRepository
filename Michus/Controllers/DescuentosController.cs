using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Michus.Service;
using System.Threading.Tasks;
using System.Security.Claims;
using Michus.Models;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Michus.Controllers
{
    [Authorize]
    public class DescuentosController : Controller
    {
        private readonly MenuService _menuService;
        private string cad_cn = "";

        public DescuentosController(MenuService menuService, IConfiguration config)
        {
            cad_cn = config.GetConnectionString("cn1");
            _menuService = menuService;
        }


        #region DESCUENTOS DAO
        public async Task<List<DescuentoCab>> GetDescuentosAsync()
        {
            var lista = new List<DescuentoCab>();

            using (var connection = new System.Data.SqlClient.SqlConnection(cad_cn))
            {
                await connection.OpenAsync();  // Abrimos la conexión asincrónicamente

                using (var command = new System.Data.SqlClient.SqlCommand("SP_LISTAR_DESCUENTOS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lista.Add(
                                new DescuentoCab()
                                {
                                    IdDescuento = dr.IsDBNull(0) ? null : dr.GetString(0), // Verificar si el valor es NULL
                                    IdPromocion = dr.IsDBNull(1) ? 0 : dr.GetInt32(1), // Verificar si el valor es NULL
                                    IdEvento = dr.IsDBNull(2) ? null : dr.GetString(2), // Verificar si el valor es NULL
                                    FechaInicio = dr.IsDBNull(3) ? DateTime.MinValue : dr.GetDateTime(3), // Verificar si el valor es NULL
                                    FechaFin = dr.IsDBNull(4) ? DateTime.MinValue : dr.GetDateTime(4), // Verificar si el valor es NULL
                                    PrecioDescuento = dr.IsDBNull(5) ? 0 : dr.GetDecimal(5), // Verificar si el valor es NULL
                                    TipoDescuento = dr.IsDBNull(6) ? (byte)0 : dr.GetByte(6), // Verificar si el valor es NULL
                                    Estado = dr.IsDBNull(7) ? 0 : dr.GetInt32(7), // Verificar si el valor es NULL
                                    TiSitu = dr.IsDBNull(8) ? null : dr.GetString(8) // Verificar si el valor es NULL
                                });
                        }
                    }
                }
            }

            return lista;
        }



        /*
        public string GrabarCliente(Clientes obj)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(
                    cad_cn, "PA_GRABAR_CLIENTE",
                    obj.cod_cli, obj.nom_cli, obj.tel_cli,
                    obj.cor_cli, obj.dir_clie, obj.cred_cli,
                    obj.fec_nac, obj.cod_dist);

                return "Se Registró/Actuaizó los datos " + $"del CLiente: {obj.nom_cli}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }
        */

        /*
        public string EliminarCliente(string codigo)
        {
            {
                try
                {
                    SqlHelper.ExecuteNonQuery(
                        cad_cn, "PA_ELIMINAR_CLIENTE",
                        codigo);

                    return "Se Eliminó de forma lógico " + $"al Cliente: {codigo}";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }


            }
        }
        */

        #endregion


        // GET: DescuentosController/Descuentos
        public static async Task<System.Data.SqlClient.SqlDataReader> ExecuteReaderAsync(string connectionString, string commandText)
        {
            var connection = new System.Data.SqlClient.SqlConnection(connectionString);
            await connection.OpenAsync();

            var command = new System.Data.SqlClient.SqlCommand(commandText, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await command.ExecuteReaderAsync();
        }


        public async Task<ActionResult> listadescuentos()
        {
            await LoadMenuDataAsync();  

            
            var descuentos = await GetDescuentosAsync();

            
            return View(descuentos);
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

        // GET: DescuentosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DescuentosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DescuentosController/Create
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

        // GET: DescuentosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DescuentosController/Edit/5
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

        // GET: DescuentosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DescuentosController/Delete/5
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
