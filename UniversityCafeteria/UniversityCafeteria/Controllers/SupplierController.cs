using Microsoft.AspNetCore.Mvc;
using UniversityCafeteria.Models;
using UniversityCafeteria.Data;

namespace UniversityCafeteria.Controllers
{
    public class SupplierController : Controller
    {
        private readonly AppDbContext _context;

        public SupplierController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var suppliers = _context.suppliers.ToList();
            return View(suppliers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
            _context.suppliers.Add(supplier);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
