namespace Exam1.Models.GET
{
    public class TicketRequestModel
    {
        //search by category name
        public string? CategoryName { get; set; }
        //search by ticket code
        public string? TicketCode { get; set; }
        //search by ticket name
        public string? TicketName { get; set; }
        //search for prices lesser than equal
        public int? Price { get; set; }
        //search by date
        public string? MinDate { get; set; }
        public string? MaxDate { get; set; }
        //order by which column
        public string? OrderBy { get; set; }
        //ascending or descending
        public string? OrderState { get; set; }
    }
}
