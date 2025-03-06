using Exam1.Models.Booking;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.Booking
{
    public class GetBookingHandler : IRequestHandler<GetBookingRequest, List<TicketsPerCategoryModel>>
    {
        private readonly BookedTicketService _service;
        public GetBookingHandler(BookedTicketService service)
        {
            _service = service;
        }

        public Task<List<TicketsPerCategoryModel>> Handle(GetBookingRequest request, CancellationToken cancellationToken)
        {
            return _service.Get(request.BookedTicketId);
        }
    }
}
