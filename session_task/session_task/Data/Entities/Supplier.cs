using System;
using System.Collections.Generic;

namespace session_task.Data.Entities;

public partial class Supplier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Offer? Offer { get; set; }
}
