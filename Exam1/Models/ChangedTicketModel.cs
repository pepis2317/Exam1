namespace Exam1.Models
{
    public class ChangedTicketModel
    {
        public required string TicketCode { get; set; }
        public required string TicketName {  get; set; }
        public required int Quantity { get; set; }
        public required string CategoryName {  get; set; }
    }
}
