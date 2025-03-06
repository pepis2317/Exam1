using Exam1.Entities;
using Exam1.Models.Booking;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Exam1.Validators.Booking
{
    public interface IValidatorForBookingSingleDelete : IValidator<BookingModel> { }
    public class BookingDeleteValidator : AbstractValidator<BookingModel>, IValidatorForBookingSingleDelete
    {
        private readonly AccelokaContext _db;

        public BookingDeleteValidator(AccelokaContext db)
        {
            _db = db;
            //RuleFor(BookedTicketId).MustAsync(ValidBookingId).WithMessage("Invalid booking id");
            RuleFor(x => x.TicketCode).MustAsync(ValidTicket).WithMessage("Invalid ticket code");
            RuleFor(x => x)
            .MustAsync(ValidCombo)
            .WithMessage("BookedTicketId and TicketCode pair doesn't exist");

            RuleFor(x => x).MustAsync(ValidQuantity).WithMessage("Quantity deleted must be less than or equal to booked quantity");

        }


        private async Task<bool> ValidTicket(string TicketCode, CancellationToken token)
        {
            var ticketData = await _db.Tickets.Where(q => q.TicketCode == TicketCode).FirstOrDefaultAsync(token);
            if (ticketData == null)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> ValidCombo(BookingModel model, CancellationToken token)
        {
            var bookingCombo = await _db.BookedTickets.Where(q => q.BookedTicketId == model.BookedTicketId).Where(q => q.TicketCode == model.TicketCode).FirstOrDefaultAsync();
            if (bookingCombo == null)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> ValidQuantity(BookingModel model, CancellationToken token)
        {
            if (model.BookedTicketId.IsNullOrEmpty())
            {
                return false;
            }
            var bookingCombo = await _db.BookedTickets.Where(q => q.BookedTicketId == model.BookedTicketId).Where(q => q.TicketCode == model.TicketCode).FirstOrDefaultAsync();
            if (bookingCombo == null)
            {
                return false;
            }
            if (model.Quantity > bookingCombo.Quantity)
            {
                return false;
            }
            return true;
        }
    }
}
