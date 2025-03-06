namespace Exam1.Models.Ticket
{
    public class TicketInfoModel
    {
        public string? TicketCode { get; set; }
        public string? TicketName { get; set; }
        public string? EventDate { get; set; }
        public int? Quantity { get; set; }
        public int? Price { get; set; }
        public int? TotalPrice { get; set; }
        public string? CategoryName { get; set; }
        public int? Quota { get; set; }
    }
}
