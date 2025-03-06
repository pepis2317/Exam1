using Exam1.Models.Booking;
using Exam1.Models.Ticket;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.Booking
{
    public class BatchEditDeleteBookingHandler : IRequestHandler<BookingIdBookingListModel, List<TicketInfoModel>>
    {
        private readonly BookedTicketService _service;
        public BatchEditDeleteBookingHandler(BookedTicketService service)
        {
            _service = service;
        }
        public Task<List<TicketInfoModel>> Handle(BookingIdBookingListModel request, CancellationToken cancellationToken)
        {
            if (request.HandlerAction == "delete")
            {
                return _service.BatchDelete(request.BookedTicketId, request.BookingList);
            }
            else if (request.HandlerAction == "edit")
            {
                return _service.Put(request.BookedTicketId, request.BookingList);
            }
            throw new NotImplementedException();

        }
    }
}
