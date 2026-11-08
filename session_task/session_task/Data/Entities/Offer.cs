using System;
using System.Collections.Generic;

namespace session_task.Data.Entities;

public partial class Offer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int SupplierId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
}
