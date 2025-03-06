using MediatR;

namespace Exam1.Models.Booking
{
    public class GetBookingRequest : IRequest<List<TicketsPerCategoryModel>>
    {
        public required string BookedTicketId { get; set; }
    }
}
