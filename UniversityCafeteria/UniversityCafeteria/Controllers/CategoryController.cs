using Microsoft.AspNetCore.Mvc;
using UniversityCafeteria.Models;
using UniversityCafeteria.Data;

namespace UniversityCafeteria.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var categories = _context.categories.ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
