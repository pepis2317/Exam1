using System;
using System.Collections.Generic;

namespace Exam1.Entities;

public partial class BookedTicket
{
    public Guid Id { get; set; }

    public string BookedTicketId { get; set; } = null!;

    public string? TicketCode { get; set; }

    public int Quantity { get; set; }

    public virtual Ticket? TicketCodeNavigation { get; set; }
}
