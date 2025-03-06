using Exam1.Entities;
using Exam1.Models.Booking;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Validators.Booking
{
    public interface IValidatorForBookingBatchDelete : IValidator<BookingIdBookingListModel> { }
    public class BookingBatchDeleteValidator : AbstractValidator<BookingIdBookingListModel>, IValidatorForBookingBatchDelete
    {
        private readonly AccelokaContext _db;
        public BookingBatchDeleteValidator(AccelokaContext db)
        {
            _db = db;
            RuleFor(x => x.BookingList).Must(NoDuplicateTickets).WithMessage("No duplicates allowed");
            RuleFor(x => x.BookingList).NotEmpty().WithMessage("At least one booking deleted");
            RuleFor(x => x.BookedTicketId).MustAsync(ValidBookingId).WithMessage("Invalid booking id");
            RuleForEach(x => x.BookingList).SetValidator(x => new BookingDeleteValidator(db));
        }
        private async Task<bool> ValidBookingId(string BookedTicketId, CancellationToken token)
        {
            var bookingData = await _db.BookedTickets.Where(q => q.BookedTicketId == BookedTicketId).FirstOrDefaultAsync(token);
            if (bookingData == null)
            {
                return false;
            }
            return true;
        }
        private bool NoDuplicateTickets(List<BookingModel> bookings)
        {
            return bookings.Select(x => x.TicketCode).Distinct().Count() == bookings.Count;
        }

    }
}
