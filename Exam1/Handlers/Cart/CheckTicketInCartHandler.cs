using Exam1.Models.Cart;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.Cart
{
    public class CheckTicketInCartHandler : IRequestHandler<CheckTicketRequest, bool>
    {
        private readonly CartService _service;
        public CheckTicketInCartHandler(CartService service)
        {
            _service = service;
        }
        public Task<bool> Handle(CheckTicketRequest request, CancellationToken cancellationToken)
        {
            return _service.checkCart(request.UserId, request.TicketCode);
        }
    }
}
