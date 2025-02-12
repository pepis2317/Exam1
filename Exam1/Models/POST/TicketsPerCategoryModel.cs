namespace Exam1.Models.POST
{
    public class TicketsPerCategoryModel
    {
        public required string CategoryName {get;set;}
        public required int SummaryPrice { get;set;}
        public required List<TicketInfo> Tickets {  get; set;}

    }
}
