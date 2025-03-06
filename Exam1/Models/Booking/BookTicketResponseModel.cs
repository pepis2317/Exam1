
using MediatR;

namespace Exam1.Models.Booking
{
    public class BookTicketResponseModel : IRequest<BookTicketResponseModel>
    {
        public required int PriceSummary { get; set; }
        public required List<TicketsPerCategoryModel> TicketsPerCategory { get; set; }
    }
}
