using Michus.DAO;
using Michus.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Michus.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ReservasDAO _reservasDAO;
        private readonly MenuService _menuService;
        private readonly IWebHostEnvironment _env;

        public ReservasController(MenuService menuService, ReservasDAO reservasDAO, IWebHostEnvironment env)
        {
            _menuService = menuService;
            _reservasDAO = reservasDAO;
            _env = env;
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

        public async Task<ActionResult> CrearReserva(string nombreUsuario, string idMesa, DateOnly fechaReserva, TimeOnly horaReserva, int cantidadPersonas)
        {
            try
            {
                // Llamamos al DAO para crear la reserva y obtener el ID_Reserva
                var idReserva = _reservasDAO.CrearReserva(nombreUsuario, idMesa, fechaReserva, horaReserva, cantidadPersonas);

                // Verificamos si el ID_Reserva no es nulo, lo que indica que la reserva se creó correctamente
                if (idReserva != null)
                {
                    // Convertir idReserva a string (si es null, se convertirá a cadena vacía "")
                    string idReservaString = idReserva.ToString(); // Esto asegura que idReserva sea un string

                    // Enviar correo de confirmación con el ID de la reserva
                    await EnviarCorreoConfirmacion(nombreUsuario, idReservaString, fechaReserva.ToDateTime(horaReserva), horaReserva, _env);

                    return Json(new { success = true, mensaje = "Reserva creada exitosamente.", idReserva = idReservaString });
                }
                else
                {
                    // Si no se pudo crear la reserva
                    return Json(new { success = false, mensaje = "Error al procesar la solicitud." });
                }
            }
            catch (Exception ex)
            {
                // En caso de error inesperado, retornamos un mensaje de error
                return StatusCode(500, new { success = false, mensaje = "Error al procesar la solicitud", error = ex.Message });
            }
        }



        public async Task EnviarCorreoConfirmacion(string correoDestino, string idReserva, DateTime fechaReserva, TimeOnly horaReserva, IWebHostEnvironment env)
        {
            try
            {
                var correoOrigen = "josejuliosanchezcruzado1@gmail.com";
                var contraseñaCorreo = "povg qncq tphk urnk";

                var asunto = "Confirmación de Reserva";

                string rutaWebRoot = env.WebRootPath;
                string rutaImagen = Path.Combine(rutaWebRoot, "assets", "img", "mishus.png");

                // Cuerpo del correo con diseño HTML
                var cuerpo = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            color: #333;
            margin: 0;
            padding: 0;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding-bottom: 20px;
        }}
        .header img {{
            max-width: 150px;
        }}
        .content {{
            text-align: left;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            font-size: 12px;
            color: #777;
            margin-top: 20px;
        }}
        .important {{
            font-weight: bold;
            color: #ff6f61;
        }}
        .details {{
            padding: 10px;
            background-color: #f0f8ff;
            border-radius: 5px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='cid:MichiLogo' alt='Logo Michi Restaurante'>
        </div>
        <div class='content'>
            <h2>¡Gracias por su reserva!</h2>
            <p>Estimado cliente,</p>
            <p>Le confirmamos que hemos recibido su reserva con los siguientes detalles:</p>
            <div class='details'>
                <p><span class='important'>ID de reserva:</span> {idReserva}</p>
                <p><span class='important'>Fecha de reserva:</span> {fechaReserva:yyyy-MM-dd}</p>
                <p><span class='important'>Hora de reserva:</span> {horaReserva.ToString(@"hh\:mm")}</p> 
            </div>
            <p>Le recordamos que si no asiste en los próximos 20 minutos luego de la hora acordadá, la mesa será liberada.</p>
        </div>
        <div class='footer'>
            <p>Gracias por elegirnos.</p>
        </div>
    </div>
</body>
</html>";

                var smtpCliente = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(correoOrigen, contraseñaCorreo),
                    EnableSsl = true
                };

                var mensaje = new MailMessage(correoOrigen, correoDestino, asunto, cuerpo)
                {
                    IsBodyHtml = true
                };

                var imagen = new Attachment(rutaImagen)
                {
                    ContentId = "MichiLogo",
                    ContentType = new System.Net.Mime.ContentType("image/png")
                };
                mensaje.Attachments.Add(imagen);

                // Enviar el correo
                await smtpCliente.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al enviar correo: {ex.Message}");
            }
        }


        [HttpPost]
        public ActionResult LiberarMesa(string idReserva)
        {
            try
            {
                // Llamar al método del DAO para liberar la mesa
                var mesaLiberada = _reservasDAO.LiberarMesa(idReserva);

                // Verificar si la mesa fue liberada exitosamente
                if (mesaLiberada is true)
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
                // En caso de error inesperado, retornamos un mensaje de error
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
