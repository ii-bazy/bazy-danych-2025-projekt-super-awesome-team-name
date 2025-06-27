using System;
using System.Collections.Generic;

namespace Online_Store.Data.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public bool ShouldSend { get; set; }

    public bool IsRead { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
