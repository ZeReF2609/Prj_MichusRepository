using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Michus.Service;
using System.Diagnostics;

namespace Michus.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginService _loginService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(LoginService loginService, ILogger<LoginController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Menu");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "El correo o la contraseña no pueden estar vacíos";
                return View();
            }

            _logger.LogDebug($"Intento de inicio de sesión: Email: {email}");

            var (message, userId, userType, role) = await _loginService.ValidateLoginAsync(email, password);

            if (message == "Bienvenido")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, role ?? "Unknown"),
                    new Claim(ClaimTypes.NameIdentifier, userId ?? "0")
                };


                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                TempData["SuccessMessage"] = "Inicio de sesión exitoso";
                return RedirectToAction("Index", "Menu");
            }

            _logger.LogWarning($"Fallo en el inicio de sesión: {message}");
            TempData["ErrorMessage"] = message;
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("Michus_Session");
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public IActionResult ResetearCookie(string cookieName)
        {
            if (!string.IsNullOrEmpty(cookieName))
            {
                Response.Cookies.Delete(cookieName);
            }
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            return View();
        }
    }
}
