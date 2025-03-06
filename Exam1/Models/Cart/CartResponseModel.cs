using Exam1.Models.Booking;
using System.ComponentModel.DataAnnotations;

namespace Exam1.Models.Cart
{
    public class CartResponseModel
    {
        public required string BookingId { get; set; }
        public required List<TicketsPerCategoryModel> BookedTickets { get; set; }
        public string? IsCompleted { get; set; }
    }
}
