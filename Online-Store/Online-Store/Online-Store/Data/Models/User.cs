using System;
using System.Collections.Generic;

namespace Online_Store.Data.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public int? PasswordId { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<OrderGroup> OrderGroups { get; set; } = new List<OrderGroup>();

    public virtual Password? Password { get; set; }

    public virtual Role? Role { get; set; }
}
