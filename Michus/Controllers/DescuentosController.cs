using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Michus.Service;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Michus.Controllers
{
    [Authorize]
    public class DescuentosController : Controller
    {
        private readonly MenuService _menuService;

        public DescuentosController(MenuService menuService)
        {
            _menuService = menuService;
        }


        // GET: DescuentosController/Descuentos
        public async Task<ActionResult> listadescuentos()
        {
            await LoadMenuDataAsync();
            return View();
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
