using System.ComponentModel.DataAnnotations;

namespace Exam1.Models.GET
{
    public class TicketModel
    {
        [Required]
        public required string EventDate { get; set; }
        [Required]
        public int Quota { get; set; }
        [Required]
        public required string TicketCode { get; set; }
        [Required]
        public required string TicketName { get; set; }
        [Required]
        public required string CategoryName { get; set; }
        [Required]
        public int Price { get; set; }

    }
}
