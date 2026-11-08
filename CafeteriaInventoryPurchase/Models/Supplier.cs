using System.ComponentModel.DataAnnotations;

namespace CafeteriaInventoryPurchase.Models;

public class Supplier
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    public string? Phone { get; set; }
    public List<Product> Products { get; set; } = new();
    public List<PurchaseOrder> PurchaseOrders { get; set; } = new();
}
