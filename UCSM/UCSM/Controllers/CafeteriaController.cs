using Microsoft.AspNetCore.Mvc;

namespace UCSM.Controllers
{
    public class CafeteriaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult Suppliers()
        {
            return View();
        }

        public IActionResult SupplyStatus()
        {
            return View();
        }
    }
}
