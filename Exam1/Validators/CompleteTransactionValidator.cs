using Exam1.Entities;
using Exam1.Models.Cart;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Validators
{
    public class CompleteTransactionValidator:AbstractValidator<CompleteTransactionRequest>
    {
        private readonly AccelokaContext _db;
        public CompleteTransactionValidator(AccelokaContext db)
        {
            _db = db;
            RuleFor(x => x.UserId).MustAsync(ValidUserId).WithMessage("Invalid user id");
            RuleFor(x => x.BookingId).MustAsync(ValidBookingId).WithMessage("Invalid booking id");
        }
        private async Task<bool> ValidUserId(Guid UserId, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (user == null)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> ValidBookingId(string bookedTicketId, CancellationToken token)
        {
            var data = await _db.BookedTickets.FirstOrDefaultAsync(x => x.BookedTicketId == bookedTicketId);
            if (data == null)
            {
                return false;
            }
            return true;
        }
    }
}
