using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Michus.Service;
using Michus.DAO;
using System.Threading.Tasks;
using System.Security.Claims;
using Michus.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Michus.Controllers
{
    [Authorize]
    public class DescuentosController : Controller
    {
        private readonly MenuService _menuService;
        private readonly DescuentosDAO _descuentosDAO;

        public DescuentosController(MenuService menuService, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("cn1");
            _menuService = menuService;
            _descuentosDAO = new DescuentosDAO(connectionString);
        }
        /*
        public async Task<ActionResult> listadescuentos()
        {
            await LoadMenuDataAsync();
            var descuentos = await _descuentosDAO.GetDescuentosAsync();
            return View(descuentos);
        }
        */

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
