using System;
using System.Collections.Generic;

namespace Exam1.Entities;

public partial class Cart
{
    public Guid CartId { get; set; }

    public Guid UserId { get; set; }

    public string BookedTicketId { get; set; } = null!;

    public string? IsCompleted { get; set; }

    public virtual User User { get; set; } = null!;
}
