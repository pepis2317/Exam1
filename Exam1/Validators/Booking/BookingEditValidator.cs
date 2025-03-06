using Exam1.Entities;
using Exam1.Models.Booking;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Validators.Booking
{

    public interface IValidatorForBookingSingleEdit : IValidator<BookingModel> { }
    public class BookingEditValidator : AbstractValidator<BookingModel>, IValidatorForBookingSingleEdit
    {
        private readonly AccelokaContext _db;

        public BookingEditValidator(AccelokaContext db)
        {
            _db = db;
            RuleFor(x => x.TicketCode).MustAsync(ValidTicket).WithMessage("Invalid ticket code");
            RuleFor(x => x).MustAsync(ValidCombo).WithMessage("BookedTicketId and TicketCode pair doesn't exist");
            RuleFor(x => x).MustAsync(ValidQuantity).WithMessage("Quantity edited must be less than or equal to ticket quota");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Booking quantity must be greater than 0");
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
            var ticketData = await _db.Tickets.Where(q => q.TicketCode == model.TicketCode).FirstOrDefaultAsync(token);
            var bookingCombo = await _db.BookedTickets.Where(q => q.BookedTicketId == model.BookedTicketId).Where(q => q.TicketCode == model.TicketCode).FirstOrDefaultAsync();
            if (bookingCombo == null || ticketData == null)
            {
                return false;
            }
            if (model.Quantity > ticketData.Quota)
            {
                return false;
            }
            return true;
        }
    }
}
