using System.ComponentModel.DataAnnotations;

namespace Exam1.Models
{
    public class BookingModel
    {
        [Required]
        public required string TicketCode { get; set; }
        [Required]
        public required int Quantity { get; set; }
    }
}
