using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace UCSM.Controllers
{
    public class SupplyRequestController : Controller
    {
        private const string DraftSessionKey = "SupplyRequestDraft";
        private const string ConfirmedRequestsKey = "ConfirmedSupplyRequests";

        private static readonly string[] ProductOptions =
        {
            "Coffee",
            "Milk",
            "Water",
            "Orange Juice",
            "Sandwich"
        };

        public IActionResult Index()
        {
            ViewBag.Products = ProductOptions;
            ViewBag.PreferredPriority = Request.Cookies["PreferredPriority"] ?? "Normal";
            ViewBag.PreferenceMessage = Request.Cookies["PreferredPriority"] is string priority
                ? $"Your preferred request priority is {priority}."
                : null;
            ViewBag.DraftItems = GetDraftItems();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(string productName, string category, int quantity, decimal unitPrice, string priority)
        {
            if (string.IsNullOrWhiteSpace(productName) ||
                !ProductOptions.Contains(productName) ||
                string.IsNullOrWhiteSpace(category) ||
                quantity <= 0 || unitPrice <= 0 ||
                !IsValidPriority(priority))
            {
                TempData["ErrorMessage"] = "Enter a valid product, category, quantity, unit price, and priority.";
                return RedirectToAction(nameof(Index));
            }

            List<SupplyRequestItem> items = GetDraftItems();
            items.Add(new SupplyRequestItem
            {
                ProductName = productName,
                Category = category,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Priority = priority
            });

            SaveDraftItems(items);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Review()
        {
            List<SupplyRequestItem> items = GetDraftItems();
            if (items.Count == 0)
            {
                TempData["ErrorMessage"] = "Add at least one product before reviewing the request.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Items = items;
            ViewBag.Subtotal = items.Sum(item => item.TotalCost);
            ViewBag.PreparationFee = CalculatePreparationFee(items);
            ViewBag.Discount = CalculateDiscount((decimal)ViewBag.Subtotal);
            ViewBag.FinalTotal = (decimal)ViewBag.Subtotal + (decimal)ViewBag.PreparationFee - (decimal)ViewBag.Discount;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmRequest()
        {
            List<SupplyRequestItem> items = GetDraftItems();
            if (items.Count == 0)
            {
                TempData["ErrorMessage"] = "There is no request to confirm.";
                return RedirectToAction(nameof(Index));
            }

            decimal subtotal = items.Sum(item => item.TotalCost);
            decimal preparationFee = CalculatePreparationFee(items);
            decimal discount = CalculateDiscount(subtotal);
            List<ConfirmedSupplyRequest> requests = GetConfirmedRequests();

            requests.Add(new ConfirmedSupplyRequest
            {
                RequestId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
                NumberOfProducts = items.Count,
                Subtotal = subtotal,
                PreparationFee = preparationFee,
                Discount = discount,
                FinalTotal = subtotal + preparationFee - discount,
                Status = "Confirmed"
            });

            HttpContext.Session.SetString(ConfirmedRequestsKey, JsonSerializer.Serialize(requests));
            HttpContext.Session.Remove(DraftSessionKey);
            TempData["Message"] = "The supply request has been confirmed.";
            return RedirectToAction(nameof(MyRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearDraft()
        {
            HttpContext.Session.Remove(DraftSessionKey);
            TempData["Message"] = "The draft request has been cleared.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePreference(string priority)
        {
            if (!IsValidPriority(priority))
            {
                TempData["ErrorMessage"] = "Choose a valid priority.";
                return RedirectToAction(nameof(Index));
            }

            Response.Cookies.Append("PreferredPriority", priority, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps
            });

            TempData["Message"] = $"Your preferred request priority is {priority}.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult MyRequests()
        {
            ViewBag.Requests = GetConfirmedRequests();
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        private List<SupplyRequestItem> GetDraftItems()
        {
            string? json = HttpContext.Session.GetString(DraftSessionKey);
            return string.IsNullOrEmpty(json)
                ? new List<SupplyRequestItem>()
                : JsonSerializer.Deserialize<List<SupplyRequestItem>>(json) ?? new List<SupplyRequestItem>();
        }

        private void SaveDraftItems(List<SupplyRequestItem> items)
        {
            HttpContext.Session.SetString(DraftSessionKey, JsonSerializer.Serialize(items));
        }

        private List<ConfirmedSupplyRequest> GetConfirmedRequests()
        {
            string? json = HttpContext.Session.GetString(ConfirmedRequestsKey);
            return string.IsNullOrEmpty(json)
                ? new List<ConfirmedSupplyRequest>()
                : JsonSerializer.Deserialize<List<ConfirmedSupplyRequest>>(json) ?? new List<ConfirmedSupplyRequest>();
        }

        private static decimal CalculatePreparationFee(List<SupplyRequestItem> items)
        {
            string priority = items.Any(item => item.Priority == "Emergency")
                ? "Emergency"
                : items.Any(item => item.Priority == "Urgent") ? "Urgent" : "Normal";

            return priority switch
            {
                "Urgent" => 5m,
                "Emergency" => 10m,
                _ => 2m
            };
        }

        private static decimal CalculateDiscount(decimal subtotal)
        {
            if (subtotal > 200m)
            {
                return subtotal * 0.15m;
            }

            if (subtotal > 100m)
            {
                return subtotal * 0.10m;
            }

            return 0m;
        }

        private static bool IsValidPriority(string priority)
        {
            return priority is "Normal" or "Urgent" or "Emergency";
        }

        public class SupplyRequestItem
        {
            public string ProductName { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public string Priority { get; set; } = "Normal";
            public decimal TotalCost => Quantity * UnitPrice;
        }

        public class ConfirmedSupplyRequest
        {
            public string RequestId { get; set; } = string.Empty;
            public int NumberOfProducts { get; set; }
            public decimal Subtotal { get; set; }
            public decimal PreparationFee { get; set; }
            public decimal Discount { get; set; }
            public decimal FinalTotal { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}
