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

        // Acción para listar todas las reservas
        [HttpGet]
        [Route("Reservas/listar-reserva")]
        public async Task<ActionResult> ListarReserva()
        {
            await LoadMenuDataAsync();

            var reservas = _reservasDAO.ListarReservas().ToList();

            var mesas = _reservasDAO.ListarMesas().ToList();

            // Depuración
            if (mesas == null)
            {
                Console.WriteLine("Las mesas son null.");
            }
            else if (mesas.Count == 0)
            {
                Console.WriteLine("No hay mesas disponibles.");
            }
            else
            {
                Console.WriteLine($"Se encontraron {mesas.Count} mesas.");
            }

            // Pasar las reservas y mesas al modelo de la vista
            ViewData["Mesas"] = mesas; // Asegúrate de pasar las mesas con "Mesas" (con mayúscula)

            return View(reservas);  // Esto devolverá una vista con la lista de reservas
        }

        // Acción para crear una reserva
        [HttpPost]
        [Route("Reservas/crear-reserva")]
        public async Task<ActionResult> CrearReserva(string idMesa, DateOnly fechaReserva, TimeOnly horaReserva, int cantidadPersonas)
        {
            await LoadMenuDataAsync();

            string nombreUsuario = GetCurrentUser(); // Obtener el nombre del usuario actual desde el claim

            // Llamar al método de DAO para crear la reserva
            bool resultado = _reservasDAO.CrearReserva(nombreUsuario, idMesa, fechaReserva, horaReserva, cantidadPersonas);

            // Verificar el resultado
            if (resultado)
            {
                // Redirigir a la lista de reservas
                return RedirectToAction("ListarReserva");
            }
            else
            {
                // En caso de error, pasar el mensaje a la vista
                ViewData["ErrorMessage"] = "No se pudo crear la reserva. Por favor, intente nuevamente.";
                return View();
            }
        }

        // Acción para liberar una mesa
        [HttpPost]
        [Route("Reservas/liberar-mesa")]
        public async Task<ActionResult> LiberarMesa(string nombreUsuario, string idMesa)
        {
            await LoadMenuDataAsync();

            // Llamar al DAO para liberar la mesa
            bool resultado = _reservasDAO.LiberarMesa(nombreUsuario, idMesa);

            // Verificar el resultado
            if (resultado)
            {
                // Redirigir a la lista de reservas
                return RedirectToAction("ListarReserva");
            }
            else
            {
                // En caso de error, pasar el mensaje a la vista
                ViewData["ErrorMessage"] = "No se pudo liberar la mesa. Por favor, intente nuevamente.";
                return View();
            }
        }

        // Acción para verificar la disponibilidad de una mesa
        [HttpGet]
        [Route("Reservas/verificar-disponibilidad")]
        public async Task<ActionResult> VerificarDisponibilidad(string idMesa)
        {
            await LoadMenuDataAsync();

            // Llamar al DAO para verificar disponibilidad
            bool resultado = _reservasDAO.VerificarDisponibilidadMesa(idMesa);

            // Pasar el resultado a la vista
            ViewData["DisponibilidadMessage"] = resultado ? "La mesa está disponible." : "La mesa no está disponible.";
            return View();
        }

        // Acción para actualizar una reserva
        [HttpPost]
        [Route("Reservas/actualizar-reserva")]
        public async Task<ActionResult> ActualizarReserva(string idReserva, DateOnly? fechaReserva, TimeOnly? horaReserva, int? cantidadPersonas)
        {
            await LoadMenuDataAsync();

            string nombreUsuario = GetCurrentUser(); // Obtener el nombre del usuario actual desde el claim

            // Llamar al DAO para actualizar la reserva
            bool resultado = _reservasDAO.ActualizarReserva(idReserva, nombreUsuario, fechaReserva, horaReserva, cantidadPersonas);

            // Verificar el resultado
            if (resultado)
            {
                // Redirigir a la lista de reservas
                return RedirectToAction("ListarReserva");
            }
            else
            {
                // En caso de error, pasar el mensaje a la vista
                ViewData["ErrorMessage"] = "No se pudo actualizar la reserva. Por favor, intente nuevamente.";
                return View();
            }
        }

        // Acción para eliminar una reserva
        [HttpPost]
        [Route("Reservas/eliminar-reserva")]
        public async Task<ActionResult> EliminarReserva(string idReserva)
        {
            await LoadMenuDataAsync();

            // Llamar al DAO para eliminar la reserva
            bool resultado = _reservasDAO.EliminarReserva(idReserva);

            // Verificar el resultado
            if (resultado)
            {
                // Redirigir a la lista de reservas
                return RedirectToAction("ListarReserva");
            }
            else
            {
                // En caso de error, pasar el mensaje a la vista
                ViewData["ErrorMessage"] = "No se pudo eliminar la reserva. Por favor, intente nuevamente.";
                return View();
            }
        }

        // Método para cargar los datos del menú
        private async Task LoadMenuDataAsync()
        {
            string roleId = GetCurrentRoleId();
            var menuJson = await _menuService.GetMenusJsonByRoleAsync(roleId);
            ViewData["MenuItems"] = string.IsNullOrEmpty(menuJson) ? "[]" : menuJson;
        }

        // Acción para listar todas las mesas
        [HttpGet]
        [Route("Reservas/listar-mesas")]
        public async Task<ActionResult> ListarMesas()
        {
            await LoadMenuDataAsync();  // Cargar el menú de datos

            // Obtener la lista de mesas desde el DAO
            var mesas = _reservasDAO.ListarMesas().ToList();

            // Pasar las mesas a la vista
            return View(mesas);
        }

        // Método para obtener el rol del usuario actual
        private string GetCurrentRoleId()
        {
            var roleIdClaim = User.FindFirst(ClaimTypes.Role);
            return roleIdClaim?.Value ?? string.Empty;
        }

        // Método para obtener el nombre del usuario actual desde los claims
        private string GetCurrentUser()
        {
            var usernameClaim = User.FindFirst(ClaimTypes.Name);
            return usernameClaim?.Value ?? string.Empty;
        }
    }
}
