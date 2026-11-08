using CafeteriaInventoryPurchase.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaInventoryPurchase.Data;

public class CafeteriaContext(DbContextOptions<CafeteriaContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Category>().Property(c => c.Name).HasMaxLength(100);
        model.Entity<Supplier>().Property(s => s.Name).HasMaxLength(100);
        model.Entity<Supplier>().Property(s => s.Email).HasMaxLength(254);
        model.Entity<Supplier>().Property(s => s.Phone).HasMaxLength(30);
        model.Entity<Product>().Property(p => p.Name).HasMaxLength(100);
        model.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        model.Entity<PurchaseOrder>().Property(o => o.Status).HasMaxLength(20);
        model.Entity<PurchaseOrderItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
        model.Entity<Product>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Products_Quantity", "[Quantity] >= 0");
            t.HasCheckConstraint("CK_Products_Price", "[Price] >= 0");
        });
        model.Entity<PurchaseOrderItem>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Items_Quantity", "[Quantity] > 0");
            t.HasCheckConstraint("CK_Items_UnitPrice", "[UnitPrice] >= 0");
        });
        model.Entity<PurchaseOrder>().ToTable(t =>
            t.HasCheckConstraint("CK_Orders_Status", "[Status] IN ('Pending', 'Received')"));

        foreach (var foreignKey in model.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
        }
    }
}
