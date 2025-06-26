using System;
using System.Collections.Generic;

namespace Online_Store.Data.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderGroupId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual OrderGroup OrderGroup { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
