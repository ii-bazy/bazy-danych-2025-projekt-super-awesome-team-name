using System;
using System.Collections.Generic;
using Online_Store.Data.Models;

namespace Online_Store.Models;

public partial class OrderGroup
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
