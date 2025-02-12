namespace Exam1.Models.GET
{
    public class BookedTicketPerCategoryModel
    {
        public required int QtyPerCategory { get; set; }
        public required string CategoryName { get; set; }
        public required List<BookedTicketModel> Tickets { get; set; }
    }
}
