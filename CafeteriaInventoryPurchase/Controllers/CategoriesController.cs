using CafeteriaInventoryPurchase.Data;
using CafeteriaInventoryPurchase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CafeteriaInventoryPurchase.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class CategoriesController(CafeteriaContext db) : Controller
{
    public IActionResult Index() => View(db.Categories.ToList());

    public IActionResult Edit(int? id)
    {
        var record = id.HasValue ? db.Categories.Find(id.Value) : new Category();
        return record is null ? NotFound() : EditView(record);
    }

    [HttpPost]
    public IActionResult Edit([Bind("Id,Name,Description")] Category input)
    {
        if (!ModelState.IsValid)
            return EditView(input);

        if (input.Id == 0)
        {
            db.Categories.Add(input);
        }
        else
        {
            var record = db.Categories.Find(input.Id);
            if (record is null)
                return NotFound();

            record.Name = input.Name;
            record.Description = input.Description;
        }

        db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var record = db.Categories.Find(id);
        if (record is null)
            return NotFound();

        db.Categories.Remove(record);
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

    private IActionResult EditView(Category record)
    {
        return View("Edit", record);
    }
}
