using Exam1.Models.Cart;
using Exam1.Services;
using FluentValidation;
using MediatR;

namespace Exam1.Handlers.Cart
{
    public class GetCartFromUserIdHandler : IRequestHandler<UserIdCart, List<CartResponseModel>>
    {
        private readonly CartService _service;
        public GetCartFromUserIdHandler(CartService service)
        {
            _service = service;
        }

        public Task<List<CartResponseModel>> Handle(UserIdCart request, CancellationToken cancellationToken)
        {
            if(request.HandlerAction == "completed")
            {
                return _service.GetCompletedTransactions(request.UserId);
            }
            else if(request.HandlerAction == "incomplete")
            {
                return _service.GetIncompleteTransactions(request.UserId);
            }
            throw new NotImplementedException();
        }
    }
}
