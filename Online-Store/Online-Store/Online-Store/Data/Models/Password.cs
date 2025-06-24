using System;
using System.Collections.Generic;

namespace Online_Store.Data.Models;

public partial class Password
{
    public int Id { get; set; }

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
