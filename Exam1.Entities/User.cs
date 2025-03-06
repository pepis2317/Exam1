using System;
using System.Collections.Generic;

namespace Exam1.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
