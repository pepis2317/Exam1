using MediatR;

namespace Exam1.Models.Booking
{
    public class UserIdBookingList : IRequest<BookTicketResponseModel>
    {
        public required Guid UserId { get; set; }
        public required List<BookingModel> BookingList { get; set; }
    }
}
