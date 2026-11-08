using Microsoft.AspNetCore.Mvc;
using UniversityCafeteria.Models;
using UniversityCafeteria.Data;

namespace UniversityCafeteria.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.products.ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
