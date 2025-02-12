using System;
using System.Collections.Generic;

namespace Exam1.Entities;

public partial class Ticket
{
    public string TicketCode { get; set; } = null!;

    public string TicketName { get; set; } = null!;

    public string CategoryId { get; set; } = null!;

    public int Price { get; set; }

    public int Quota { get; set; }

    public DateTime EventDate { get; set; }

    public virtual ICollection<BookedTicket> BookedTickets { get; set; } = new List<BookedTicket>();

    public virtual Category Category { get; set; } = null!;
}
