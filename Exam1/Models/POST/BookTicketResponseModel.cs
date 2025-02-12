namespace Exam1.Models.POST
{
    public class BookTicketResponseModel
    {
        public required int PriceSummary { get; set; }
        public required List<TicketsPerCategoryModel> TicketsPerCategory {get; set;}
    }
}
