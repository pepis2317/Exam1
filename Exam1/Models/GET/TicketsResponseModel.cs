namespace Exam1.Models.GET
{
    public class TicketsResponseModel
    {
        public required List<TicketModel> Tickets { get; set; }
        public int TotalTickets { get; set; }
    }
}
