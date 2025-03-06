using MediatR;

namespace Exam1.Models.Cart
{
    public class CheckTicketRequest : IRequest<bool>
    {
        public required Guid UserId { get; set; }
        public required string TicketCode { get; set; }
    }
}
