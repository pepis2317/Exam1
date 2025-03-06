using Exam1.Models.Booking;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.Booking
{
    public class PostBookingHandler : IRequestHandler<UserIdBookingList, BookTicketResponseModel>
    {
        private readonly BookedTicketService _service;
        public PostBookingHandler(BookedTicketService service)
        {
            _service = service;
        }

        public Task<BookTicketResponseModel> Handle(UserIdBookingList request, CancellationToken cancellationToken)
        {
            return _service.Post(request.UserId, request.BookingList);
        }
    }
}
