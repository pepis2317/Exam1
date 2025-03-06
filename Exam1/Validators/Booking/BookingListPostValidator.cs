using DocumentFormat.OpenXml.Spreadsheet;
using Exam1.Entities;
using Exam1.Models.Booking;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Validators.Booking
{
    public interface IValidatorForBookingListPost : IValidator<UserIdBookingList> { }
    public class BookingListPostValidator : AbstractValidator<UserIdBookingList>, IValidatorForBookingListPost
    {
        private readonly AccelokaContext _db;
        public BookingListPostValidator(AccelokaContext db)
        {
            _db = db;
            RuleFor(x => x.UserId).MustAsync(CheckUser).WithMessage("User doesn't exist");
            RuleFor(x => x.BookingList).NotEmpty().WithMessage("At least one booking edited");
            RuleFor(x => x.BookingList).Must(NoDuplicateTickets).WithMessage("No duplicates allowed");
            RuleForEach(x => x.BookingList).SetValidator(new BookingPostValidator(db));
        }
        private async Task<bool> CheckUser(Guid UserId, CancellationToken token)
        {
            var userData = await _db.Users.Where(q => q.UserId == UserId).FirstOrDefaultAsync(token);
            if (userData == null)
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
