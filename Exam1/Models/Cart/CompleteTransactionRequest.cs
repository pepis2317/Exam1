using MediatR;

namespace Exam1.Models.Cart
{
    public class CompleteTransactionRequest : IRequest<CartResponseModel>
    {
        public required Guid UserId { get; set; }
        public required string BookingId { get; set; }
    }
}
