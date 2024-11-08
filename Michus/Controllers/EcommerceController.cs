using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Michus.Controllers
{
    public class EcommerceController : Controller
    {
        // GET: EcommerceController
        public ActionResult EcommerceProductosIndex()
        {
            return View();
        }

    }
}
