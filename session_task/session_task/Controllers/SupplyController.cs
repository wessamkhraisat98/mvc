
using Microsoft.AspNetCore.Mvc;
using session_task.Models;
using System.Text.Json;

namespace session_task.Controllers
{
    public class SupplyController : Controller
    {
        private static readonly List<Category> categories = new()
        {
            new Category { Id = 1, Name = "Graduation Certificates" },
            new Category { Id = 2, Name = "Hospitality" },
            new Category { Id = 3, Name = "Graduation Gifts" }
        };

        private static readonly List<Supplier> suppliers = new()
        {
            new Supplier { Id = 1, Name = "Supplier A", CategoryId = 1 },
            new Supplier { Id = 2, Name = "Supplier B", CategoryId = 1 },
            new Supplier { Id = 3, Name = "Supplier C", CategoryId = 1 },

            new Supplier { Id = 4, Name = "Supplier D", CategoryId = 2 },
            new Supplier { Id = 5, Name = "Supplier E", CategoryId = 2 },
            new Supplier { Id = 6, Name = "Supplier F", CategoryId = 2 },

            new Supplier { Id = 7, Name = "Supplier G", CategoryId = 3 },
            new Supplier { Id = 8, Name = "Supplier H", CategoryId = 3 },
            new Supplier { Id = 9, Name = "Supplier I", CategoryId = 3 }
        };

        private static readonly List<Offer> offers = new()
        {
            new Offer { Id = 1, Name = "Premium Certificate Package", SupplierId = 1, Quantity = 35, UnitPrice = 2m },
            new Offer { Id = 2, Name = "Standard Certificate Package", SupplierId = 2, Quantity = 35, UnitPrice = 1.5m },
            new Offer { Id = 3, Name = "Luxury Certificate Package", SupplierId = 3, Quantity = 35, UnitPrice = 2.5m },

            new Offer { Id = 4, Name = "Coffee & Drinks Package", SupplierId = 4, Quantity = 100, UnitPrice = 2m },
            new Offer { Id = 5, Name = "Sandwiches & Snacks Package", SupplierId = 5, Quantity = 100, UnitPrice = 3m },
            new Offer { Id = 6, Name = "Full Hospitality Package", SupplierId = 6, Quantity = 100, UnitPrice = 4m },

            new Offer { Id = 7, Name = "Customized Mugs Package", SupplierId = 7, Quantity = 35, UnitPrice = 2m },
            new Offer { Id = 8, Name = "Graduation Bags Package", SupplierId = 8, Quantity = 35, UnitPrice = 3m },
            new Offer { Id = 9, Name = "Premium Gift Boxes Package", SupplierId = 9, Quantity = 35, UnitPrice = 4m }
        };

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Categories = categories;
            ViewBag.Suppliers = suppliers;
            ViewBag.Offers = offers;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Dictionary<int, int> selectedOffers)
        {
            if (selectedOffers == null ||
                selectedOffers.Count != categories.Count)
            {
                return BadRequest("Please select one offer from each category.");
            }

            List<Offer> selectedOfferList = new List<Offer>();

            foreach (var category in categories)
            {
                if (!selectedOffers.TryGetValue(category.Id, out int offerId))
                {
                    return BadRequest("Missing category selection.");
                }

                var offer = offers.FirstOrDefault(o => o.Id == offerId);

                if (offer == null)
                {
                    return BadRequest("Invalid offer.");
                }

                var supplier = suppliers.FirstOrDefault(
                    s => s.Id == offer.SupplierId);

                if (supplier == null || supplier.CategoryId != category.Id)
                {
                    return BadRequest("Offer does not belong to this category.");
                }

                selectedOfferList.Add(offer);
            }

            string json = JsonSerializer.Serialize(selectedOfferList);

            HttpContext.Session.SetString("SelectedOffers", json);

            return RedirectToAction("Review");
        }

        [HttpGet]
        public IActionResult Review()
        {
            string? json = HttpContext.Session.GetString("SelectedOffers");

            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("Index");
            }

            List<Offer>? selectedOffers;

            try
            {
                selectedOffers = JsonSerializer.Deserialize<List<Offer>>(json);
            }
            catch (JsonException)
            {
                return RedirectToAction("Index");
            }

            if (selectedOffers == null ||
                selectedOffers.Count != categories.Count)
            {
                return RedirectToAction("Index");
            }

            foreach (var category in categories)
            {
                int matchingOffers = selectedOffers.Count(offer =>
                {
                    var supplier = suppliers.FirstOrDefault(
                        s => s.Id == offer.SupplierId);

                    return offers.Any(o =>
                        o.Id == offer.Id &&
                        o.SupplierId == offer.SupplierId &&
                        o.Quantity == offer.Quantity &&
                        o.UnitPrice == offer.UnitPrice &&
                        o.Name == offer.Name)
                        && supplier != null
                        && supplier.CategoryId == category.Id;
                });

                if (matchingOffers != 1)
                {
                    return RedirectToAction("Index");
                }
            }

            decimal totalPrice = 0;

            Dictionary<int, decimal> offerTotals = new();

            foreach (var offer in selectedOffers)
            {
                decimal offerTotal = offer.Quantity * offer.UnitPrice;

                offerTotals.Add(offer.Id, offerTotal);

                totalPrice += offerTotal;
            }

            decimal discountPercentage;

            if (totalPrice > 500)
            {
                discountPercentage = 15;
            }
            else if (totalPrice > 300)
            {
                discountPercentage = 10;
            }
            else
            {
                discountPercentage = 0;
            }

            decimal discountAmount =
                totalPrice * discountPercentage / 100;

            decimal finalTotal = totalPrice - discountAmount;

            ViewBag.Categories = categories;
            ViewBag.Suppliers = suppliers;
            ViewBag.SelectedOffers = selectedOffers;
            ViewBag.OfferTotals = offerTotals;

            ViewBag.TotalPrice = totalPrice;
            ViewBag.DiscountPercentage = discountPercentage;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.FinalTotal = finalTotal;

            return View();
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
    }
}
