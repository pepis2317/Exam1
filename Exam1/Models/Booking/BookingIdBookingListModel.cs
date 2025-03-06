using Exam1.Models.Ticket;
using MediatR;

namespace Exam1.Models.Booking
{
    public class BookingIdBookingListModel : IRequest<List<TicketInfoModel>>
    {
        public string? HandlerAction { get; set; }
        public string BookedTicketId { get; set; }
        public List<BookingModel> BookingList { get; set; }

        public BookingIdBookingListModel(string bookedTicketId, List<BookingModel> BookingList)
        {
            BookedTicketId = bookedTicketId;
            this.BookingList = BookingList;
            foreach (var booking in this.BookingList)
            {
                booking.BookedTicketId = bookedTicketId;
            }
        }
    }

}
