using Exam1.Models.Cart;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.Cart
{
    public class CompleteTransactionHandler : IRequestHandler<CompleteTransactionRequest, CartResponseModel>
    {
        private readonly CartService _service;
        public CompleteTransactionHandler(CartService service)
        {
            _service = service;
        }
        public Task<CartResponseModel> Handle(CompleteTransactionRequest request, CancellationToken cancellationToken)
        {
            return _service.CompleteTransaction(request.UserId, request.BookingId);
        }
    }
}
