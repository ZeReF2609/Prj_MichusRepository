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
        public IActionResult RegistrarCliente(ClienteRegistroModel model)
        {


            ViewBag.TipoDocumento = _loginCliService.ObtenerTiposDocumentoAsync().Result;
            try
            {
                _logger.LogInformation(model.ToString());

                // Validación de campos nulos y asignación de valores predeterminados
                var idCliente = model.IdCliente ?? Guid.NewGuid().ToString();
                var email = model.Emailre ?? string.Empty;
                var contrasenia = model.Passwordre ?? string.Empty;
                var nombres = model.Nombres ?? "Sin Nombre";
                var apellidos = model.Apellidos ?? "Sin Apellidos";
                var idDoc = model.IdDoc != 0 ? model.IdDoc : -1; // Asignar un ID de documento predeterminado si es 0
                var docIdent = model.DocIdent ?? "Sin Documento";
                var fechaNacimiento = model.FechaNacimiento != DateTime.MinValue ? model.FechaNacimiento : DateTime.Now;

                // Llamada al servicio con parámetros
                bool isRegistered = _loginCliService.RegisterClientAsync(
                    idCliente,         // ID_CLIENTE
                    email,             // EMAIL
                    contrasenia,       // CONTRASENIA
                    nombres,           // NOMBRES
                    apellidos,         // APELLIDOS
                    idDoc,             // ID_DOC
                    docIdent,          // DOC_IDENT
                    fechaNacimiento,   // FECHA_NACIMIENTO
                    1                  // ACCION
                );

                if (isRegistered)
                {
                    _logger.LogInformation("Registro exitoso para usuario");
                    TempData["SuccessMessage"] = "Registro exitoso. Por favor, inicie sesión.";
                    return RedirectToAction("LoginCli", "LoginCli");
                }

                _logger.LogWarning("Fallo en el registro");
                TempData["ErrorMessage"] = "No se pudo completar el registro.";
                ViewBag.TipoDocumento = _loginCliService.ObtenerTiposDocumentoAsync().Result;
                return View("LoginCli");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error no controlado en registro: {ex}");
                TempData["ErrorMessage"] = "Ha ocurrido un error inesperado. Por favor, intente nuevamente.";
                ViewBag.TipoDocumento = _loginCliService.ObtenerTiposDocumentoAsync().Result;
                return View("LoginCli");
            }
        }


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

            // Verificar el mensaje de respuesta del servicio
            if (message != "Inicio de sesión exitoso")
            {
                TempData["ErrorMessage"] = message;
                return View();
            }

            // Verificar si el userId o el rol son nulos o vacíos
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            {
                TempData["ErrorMessage"] = "Error en los datos del usuario, por favor intente nuevamente.";
                return View();
            }

            // Si el usuario es válido, se crea la sesión
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Name, email),
        new Claim(ClaimTypes.Role, role)
    };

            ViewBag.email = email;

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Iniciar sesión con la cookie de autenticación
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Redirigir a la página de productos después de un inicio de sesión exitoso
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
