using CafeteriaInventoryPurchase.Data;
using CafeteriaInventoryPurchase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CafeteriaInventoryPurchase.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class SuppliersController(CafeteriaContext db) : Controller
{
    public IActionResult Index() => View(db.Suppliers.ToList());

    public IActionResult Edit(int? id)
    {
        var record = id.HasValue ? db.Suppliers.Find(id.Value) : new Supplier();
        return record is null ? NotFound() : EditView(record);
    }

    [HttpPost]
    public IActionResult Edit([Bind("Id,Name,Email,Phone")] Supplier input)
    {
        if (!ModelState.IsValid)
            return EditView(input);

        if (input.Id == 0)
        {
            db.Suppliers.Add(input);
        }
        else
        {
            var record = db.Suppliers.Find(input.Id);
            if (record is null)
                return NotFound();

            record.Name = input.Name;
            record.Email = input.Email;
            record.Phone = input.Phone;
        }

        db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var record = db.Suppliers.Find(id);
        if (record is null)
            return NotFound();

        db.Suppliers.Remove(record);
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

    private IActionResult EditView(Supplier record)
    {
        return View("Edit", record);
    }
}
