using Michus.DAO;
using Michus.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Michus.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ReservasDAO _reservasDAO;
        private readonly MenuService _menuService;

        public ReservasController(MenuService menuService, ReservasDAO reservasDAO)
        {
            _menuService = menuService;
            _reservasDAO = reservasDAO;
        }

        [HttpGet]
        [Route("Reservas/listar-reserva")]
        public async Task<ActionResult> ListarReserva()
        {
            var reservas = _reservasDAO.ListarReservas();

            var mesas = ListarMesas();

            await LoadMenuDataAsync();

            ViewData["Mesas"] = mesas;

            return View(reservas);
        }

        public ActionResult CrearReserva(string nombreUsuario, string idMesa, DateOnly fechaReserva, TimeOnly horaReserva, int cantidadPersonas)
        {
            try
            {
                // Llamamos al DAO para crear la reserva
                var reservaCreada = _reservasDAO.CrearReserva(nombreUsuario, idMesa, fechaReserva, horaReserva, cantidadPersonas);

                // Si la reserva se creó correctamente
                if (reservaCreada)
                {
                    return Json(new { success = true, mensaje = "Reserva creada exitosamente." });
                }
                else
                {
                    // Si la reserva no se creó correctamente
                    return Json(new { success = false, mensaje = "Error al procesar la solicitud." });
                }
            }
            catch (Exception ex)
            {
                // En caso de un error inesperado, retornamos un mensaje de error
                return StatusCode(500, new { success = false, mensaje = "Error al procesar la solicitud", error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult LiberarMesa(string idReserva)
        {
            try
            {
                // Llamar al método del DAO para liberar la mesa
                var mesaLiberada = _reservasDAO.LiberarMesa(idReserva);

                if (mesaLiberada)
                {
                    return Json(new { success = true, mensaje = "Mesa liberada exitosamente." });
                }
                else
                {
                    return Json(new { success = false, mensaje = "Error al liberar la mesa." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, mensaje = "Error al procesar la solicitud", error = ex.Message });
            }
        }

        public ActionResult ListarMesas()
        {
            var mesas = _reservasDAO.ListarMesas();

            return Json(mesas);
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
