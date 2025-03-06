using MediatR;

namespace Exam1.Models.Cart
{
    public class UserIdCart : IRequest<List<CartResponseModel>>
    {
        public required Guid UserId { get; set; }
        public string? HandlerAction { get; set; }
    }
}
