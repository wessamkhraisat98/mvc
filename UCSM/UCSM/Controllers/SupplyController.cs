using Microsoft.AspNetCore.Mvc;

namespace UCSM.Controllers
{
    public class SupplyController : Controller
    {
        public IActionResult Index()
        {
            string[] categories = { "Drinks", "Food", "Snacks", "Hot Drinks", "Cold Drinks" };
            ViewBag.Categories = categories;
            return View();
        }

        public IActionResult Products()
        {
            ViewData["Title"] = "Cafeteria Products";
            ViewData["ManagerName"] = "Ahmad Ali";
            ViewData["NumberOfProducts"] = 5;
            ViewData["Products"] = new[] { "Water", "Orange Juice", "Coffee", "Sandwich", "Chocolate" };
            return View();
        }

        public IActionResult Suppliers()
        {
            ViewBag.Title = "Our Suppliers";
            ViewBag.NumberOfSuppliers = 3;
            ViewBag.Suppliers = new[]
            {
                "Fresh Food Company",
                "Happy Drinks Company",
                "Daily Supplies Company"
            };
            return View();
        }

        [HttpGet]
        public IActionResult CreateRequest()
        {
            ViewBag.Products = new[] { "Water", "Orange Juice", "Coffee", "Sandwich", "Chocolate" };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitRequest()
        {
            string employeeName = Request.Form["EmployeeName"].ToString().Trim();
            string productName = Request.Form["ProductName"].ToString();
            string quantityValue = Request.Form["Quantity"].ToString();
            string requiredDate = Request.Form["RequiredDate"].ToString();

            if (string.IsNullOrWhiteSpace(employeeName))
            {
                ViewBag.ErrorMessage = "Please enter the employee name.";
                ViewBag.Products = new[] { "Water", "Orange Juice", "Coffee", "Sandwich", "Chocolate" };
                ViewBag.ProductName = productName;
                ViewBag.Quantity = quantityValue;
                ViewBag.RequiredDate = requiredDate;
                return View("CreateRequest");
            }

            if (!int.TryParse(quantityValue, out int quantity) || quantity < 1 ||
                string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(requiredDate))
            {
                ViewBag.ErrorMessage = "Please provide a product, a quantity greater than zero, and a required date.";
                ViewBag.Products = new[] { "Water", "Orange Juice", "Coffee", "Sandwich", "Chocolate" };
                ViewBag.EmployeeName = employeeName;
                ViewBag.ProductName = productName;
                ViewBag.Quantity = quantityValue;
                ViewBag.RequiredDate = requiredDate;
                return View("CreateRequest");
            }

            TempData["Message"] = $"Supply request submitted successfully for {employeeName}.";
            return RedirectToAction("About");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult CheckQuantity(int quantity)
        {
            ViewBag.Quantity = quantity;
            ViewBag.Classification = quantity < 20
                ? "Small Order"
                : quantity <= 50 ? "Medium Order" : "Large Order";
            return View();
        }
    }
}
