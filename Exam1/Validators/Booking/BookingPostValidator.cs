using Exam1.Entities;
using Exam1.Models.Booking;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Validators.Booking
{
    public interface IValidatorForBookingSinglePost : IValidator<BookingModel> { }
    public class BookingPostValidator : AbstractValidator<BookingModel>, IValidatorForBookingSinglePost

    {
        private readonly AccelokaContext _db;
        public BookingPostValidator(AccelokaContext db)
        {
            _db = db;
            RuleFor(x => x.TicketCode).MustAsync(CheckTicket).WithMessage("Invalid ticket code").MustAsync(ValidDate).WithMessage("Ticket cannot be past event");

            RuleFor(x => x).MustAsync(ValidQuota).WithMessage("Cannot be booked due to available quota");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Booking quantity must be greater than 0");

        }
        private async Task<bool> CheckTicket(string ticketCode, CancellationToken token)
        {
            var ticket = await _db.Tickets.Where(q => q.TicketCode == ticketCode).FirstOrDefaultAsync(token);
            if (ticket == null)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> ValidDate(string ticketCode, CancellationToken token)
        {
            var ticket = await _db.Tickets.Where(q => q.TicketCode == ticketCode).FirstOrDefaultAsync(token);
            if (ticket == null)
            {
                return false;
            }
            if (ticket.EventDate <= DateTime.Now)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> ValidQuota(BookingModel booking, CancellationToken token)
        {
            var ticket = await _db.Tickets.Where(q => q.TicketCode == booking.TicketCode).FirstOrDefaultAsync(token);
            if (ticket == null)
            {
                return false;
            }
            if (ticket.Quota == 0 || ticket.Quota < booking.Quantity)
            {
                return false;
            }
            return true;
        }
    }
}
