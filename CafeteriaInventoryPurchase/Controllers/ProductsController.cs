using CafeteriaInventoryPurchase.Data;
using CafeteriaInventoryPurchase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CafeteriaInventoryPurchase.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class ProductsController(CafeteriaContext db) : Controller
{
    public IActionResult Index() => View(db.Products.Include(p => p.Category).Include(p => p.Supplier).ToList());

    public IActionResult Edit(int? id)
    {
        var record = id.HasValue ? db.Products.Find(id.Value) : new Product();
        return record is null ? NotFound() : EditView(record);
    }

    [HttpPost]
    public IActionResult Edit([Bind("Id,Name,Price,Quantity,CategoryId,SupplierId")] Product input)
    {
        if (!db.Categories.Any(c => c.Id == input.CategoryId))
            ModelState.AddModelError("CategoryId", "Select a category.");
        if (!db.Suppliers.Any(s => s.Id == input.SupplierId))
            ModelState.AddModelError("SupplierId", "Select a supplier.");
        if (!ModelState.IsValid)
            return EditView(input);

        if (input.Id == 0)
        {
            db.Products.Add(input);
        }
        else
        {
            var record = db.Products.Find(input.Id);
            if (record is null)
                return NotFound();

            record.Name = input.Name;
            record.Price = input.Price;
            record.Quantity = input.Quantity;
            record.CategoryId = input.CategoryId;
            record.SupplierId = input.SupplierId;
        }

        db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var record = db.Products.Find(id);
        if (record is null)
            return NotFound();

        db.Products.Remove(record);
        try
        {
            db.SaveChanges();
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "This record is in use and cannot be deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    private IActionResult EditView(Product record)
    {
        ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
        ViewBag.Suppliers = new SelectList(db.Suppliers.ToList(), "Id", "Name");
        return View("Edit", record);
    }
}
