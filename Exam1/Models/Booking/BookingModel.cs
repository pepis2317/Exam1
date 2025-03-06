using Exam1.Models.Ticket;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Exam1.Models.Booking
{
    public class BookingModel : IRequest<List<TicketInfoModel>>
    {
        [Required]
        public required string TicketCode { get; set; }
        [Required]
        public required int Quantity { get; set; }
        [JsonIgnore]
        public string? BookedTicketId { get; set; }
    }
}
