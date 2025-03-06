using Exam1.Models.Booking;
using Exam1.Models.Ticket;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.Booking
{
    public class DeleteBookingHandler : IRequestHandler<BookingModel, List<TicketInfoModel>>
    {
        private readonly BookedTicketService _service;
        public DeleteBookingHandler(BookedTicketService service)
        {
            _service = service;
        }

        public Task<List<TicketInfoModel>> Handle(BookingModel request, CancellationToken cancellationToken)
        {
            if (request.BookedTicketId != null)
            {
                return _service.Delete(request.BookedTicketId, request.TicketCode, request.Quantity);

            }
            throw new NotImplementedException();
        }
    }
}
