using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Michus.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Michus.Models;

namespace Michus.Controllers
{
    public class LoginCliController : Controller
    {
        private readonly LoginCliService _loginCliService;
        private readonly ILogger<LoginCliController> _logger;

        public LoginCliController(LoginCliService loginCliService, ILogger<LoginCliController> logger)
        {
            _loginCliService = loginCliService;
            _logger = logger;
        }

        // Vista de inicio de sesión y registro combinados
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LoginCli()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction("ListarProductos", "Ecommerce");
                }

                var tiposDocumento = await _loginCliService.ObtenerTiposDocumentoAsync();

                // Verify if we got any data
                if (tiposDocumento == null || !tiposDocumento.Any())
                {
                    _logger.LogWarning("No se encontraron tipos de documento");
                    tiposDocumento = new List<TipoDocumento>(); // Initialize empty list to prevent null reference
                }

                ViewBag.TipoDocumento = tiposDocumento;

                // Update the log to show meaningful information
                _logger.LogDebug($"Tipos de documento cargados: {tiposDocumento.Count}");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en LoginCli: {ex.Message}");
                // Handle the error appropriately
                ViewBag.Error = "Ha ocurrido un error al cargar los tipos de documento.";
                ViewBag.TipoDocumento = new List<TipoDocumento>(); // Initialize empty list to prevent null reference
                return View();
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegistrarCliente(ClienteRegistroModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Por favor, corrija los siguientes errores: " + string.Join(", ", errors);
                // Recargar los tipos de documento para la vista
                ViewBag.TipoDocumento = await _loginCliService.ObtenerTiposDocumentoAsync();
            }

            try
            {
                _logger.LogInformation(model.ToString());

                var (message, userId) = await _loginCliService.RegisterClientAsync(
                    model.Email,
                    model.Password,
                    model.Nombres,
                    model.Apellidos,
                    "cliente", // userType
                    "User",    // role
                    "/images/default-avatar.png", // avatarUrl
                    model.IdCliente,
                    model.IdDoc,
                    model.DocIdent,
                    model.FechaNacimiento);


                if (userId != null)
                {
                    _logger.LogInformation($"Registro exitoso para usuario ID: {userId}");
                    TempData["SuccessMessage"] = "Registro exitoso. Por favor, inicie sesión.";
                    return RedirectToAction("LoginCli", "LoginCli");
                }

                _logger.LogWarning($"Fallo en el registro: {message}");
                TempData["ErrorMessage"] = message;
                ViewBag.TipoDocumento = await _loginCliService.ObtenerTiposDocumentoAsync();
                return View("LoginCli");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error no controlado en registro: {ex}");
                TempData["ErrorMessage"] = "Ha ocurrido un error inesperado. Por favor, intente nuevamente.";
                ViewBag.TipoDocumento = await _loginCliService.ObtenerTiposDocumentoAsync();
                return View("LoginCli");
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginCli(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Por favor, ingrese el correo y la contraseña.";
                return View();
            }

            _logger.LogDebug($"Intento de inicio de sesión: Email: {email}");

            // Llamada al servicio para validar el usuario
            var (message, userId, role) = await _loginCliService.ValidarUsuarioAsync(email, password);

            if (!string.IsNullOrEmpty(message))
            {
                TempData["ErrorMessage"] = message;
                return View();
            }

            // Si el usuario es válido, se crea la sesión
            var claims = new List<Claim>{
        new Claim(ClaimTypes.NameIdentifier, userId!.ToString()),
        new Claim(ClaimTypes.Name, email),
        new Claim(ClaimTypes.Role, role)


        };
            ViewBag.email = email;

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("ListarProductos", "Ecommerce");
        }




        [AllowAnonymous]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("Michus_Session");
            return RedirectToAction("LoginCli", "LoginCli");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PerfilCliente()
        {
            try
                {
                // Obtener el ID del cliente desde los claims
                var clienteId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(clienteId))
                {
                    TempData["ErrorMessage"] = "No se pudo identificar al cliente. Por favor, inicie sesión.";
                    return RedirectToAction("LoginCli");
                }
                if (clienteId.StartsWith("U"))
                {
                    clienteId = "C" + clienteId.Substring(1);
                }
                // Obtener el cliente usando el servicio
                var cliente = await _loginCliService.ObtenerClientePorIdAsync(clienteId);

                if (cliente == null)
                {
                    TempData["ErrorMessage"] = "No se encontró el cliente.";
                    return RedirectToAction("ListarProductos", "Ecommerce");
                }

                // Cargar tipos de documento si se editan
                var tiposDocumento = await _loginCliService.ObtenerTiposDocumentoAsync();
                ViewBag.TipoDocumento = tiposDocumento;

                return View(cliente); // Asegúrate de que la vista coincida con el modelo devuelto
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cargar el perfil: {ex}");
                TempData["ErrorMessage"] = "Ocurrió un error al cargar el perfil.";
                return RedirectToAction("ListarProductos", "Ecommerce");
            }
        }




    }
}
