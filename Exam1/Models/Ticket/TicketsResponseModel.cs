namespace Exam1.Models.Ticket
{
    public class TicketsResponseModel
    {
        public required List<TicketInfoModel> Tickets { get; set; }
        public int TotalTickets { get; set; }
    }
}
