using Exam1.Models.Ticket;

namespace Exam1.Models.Booking
{
    public class TicketsPerCategoryModel
    {
        public int? QtyPerCategory { get; set; }
        public required string CategoryName { get; set; }
        public required int SummaryPrice { get; set; }
        public required List<TicketInfoModel> Tickets { get; set; }

    }
}
