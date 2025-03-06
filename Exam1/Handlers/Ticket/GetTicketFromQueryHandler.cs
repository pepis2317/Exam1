using Exam1.Models.Ticket;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.Ticket
{
    public class GetTicketFromQueryHandler : IRequestHandler<TicketRequestModel, TicketsResponseModel>
    {
        private readonly TicketService _service;
        public GetTicketFromQueryHandler(TicketService service)
        {
            _service = service;
        }
        public Task<TicketsResponseModel> Handle(TicketRequestModel request, CancellationToken cancellationToken)
        {
            return _service.Get(request);
        }
    }
}
