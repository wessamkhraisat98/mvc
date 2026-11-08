using System.Data;
using CafeteriaInventoryPurchase.Data;
using CafeteriaInventoryPurchase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaInventoryPurchase.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class PurchaseOrdersController(CafeteriaContext db) : Controller
{
    public IActionResult Index() => View(db.PurchaseOrders.Include(o => o.Supplier)
        .OrderByDescending(o => o.Id).ToList());

    public IActionResult Create() => View(db.Suppliers.ToList());

    [HttpPost]
    public IActionResult Create(int supplierId)
    {
        if (!db.Suppliers.Any(s => s.Id == supplierId))
        {
            ModelState.AddModelError("", "Select a supplier.");
            return View(db.Suppliers.ToList());
        }

        var order = new PurchaseOrder { SupplierId = supplierId };
        db.PurchaseOrders.Add(order);
        db.SaveChanges();
        return RedirectToAction(nameof(Details), new { id = order.Id });
    }

    public IActionResult Details(int id)
    {
        var order = db.PurchaseOrders.Include(o => o.Supplier)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .SingleOrDefault(o => o.Id == id);
        if (order is null)
            return NotFound();

        ViewBag.Products = db.Products.Where(p => p.SupplierId == order.SupplierId).ToList();
        ViewBag.Total = order.Items.Sum(i => i.Quantity * i.UnitPrice);
        return View(order);
    }

    [HttpPost]
    public IActionResult AddItem(int id, int productId, int quantity)
    {
        using var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable);
        var order = db.PurchaseOrders.Include(o => o.Items).SingleOrDefault(o => o.Id == id);
        if (order is null)
            return NotFound();

        var product = db.Products.Find(productId);
        if (order.Status != "Pending" || product is null ||
            product.SupplierId != order.SupplierId || quantity <= 0)
        {
            TempData["Error"] = "Choose a product from this supplier and enter a positive quantity on a pending order.";
            return RedirectToAction(nameof(Details), new { id });
        }

        order.Items.Add(new PurchaseOrderItem
        {
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = product.Price
        });
        db.SaveChanges();
        transaction.Commit();
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public IActionResult Receive(int id)
    {
        using var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable);
        var order = db.PurchaseOrders.Include(o => o.Items).ThenInclude(i => i.Product)
            .SingleOrDefault(o => o.Id == id);
        if (order is null)
            return NotFound();

        if (order.Status != "Pending" || order.Items.Count == 0)
        {
            TempData["Error"] = "Only pending orders with products can be received.";
            return RedirectToAction(nameof(Details), new { id });
        }

        foreach (var items in order.Items.GroupBy(i => i.ProductId))
        {
            var product = items.First().Product!;
            long quantity = product.Quantity + items.Sum(i => (long)i.Quantity);
            if (quantity > int.MaxValue)
            {
                TempData["Error"] = "The resulting stock quantity is too large.";
                return RedirectToAction(nameof(Details), new { id });
            }

            product.Quantity = (int)quantity;
        }

        order.Status = "Received";
        db.SaveChanges();
        transaction.Commit();
        return RedirectToAction(nameof(Details), new { id });
    }
}
