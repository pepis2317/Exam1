using Exam1.Models.Ticket;
using FluentValidation;
using System.Globalization;

namespace Exam1.Validators
{
    public class TicketRequestValidator : AbstractValidator<TicketRequestModel>
    {
        private static readonly HashSet<string> ValidColumns = new()
        {
            "CategoryName", "TicketCode", "TicketName", "EventDate", "Price", "Quota"
        };
        public TicketRequestValidator()
        {
            RuleFor(x => x.MinDate).Must(minDate => string.IsNullOrEmpty(minDate) || ValidDateFormat(minDate)).WithMessage("Min date is not in the correct format (dd-MM-yyyy HH:mm)");
            RuleFor(x => x.MinDate).Must(maxDate => string.IsNullOrEmpty(maxDate) || ValidDateFormat(maxDate)).WithMessage("Max date is not in the correct format (dd-MM-yyyy HH:mm)");
            RuleFor(x => x.OrderBy).Must(orderBy => orderBy == null || ValidColumns.Contains(orderBy)).WithMessage(x => $"{x.OrderBy} is not a valid column name (column names are case sensitive)");
            RuleFor(x => x.OrderState).Must(orderState => orderState == null || orderState.Equals("ascending", StringComparison.OrdinalIgnoreCase) || orderState.Equals("descending", StringComparison.OrdinalIgnoreCase)).WithMessage("Order state must be 'ascending' or 'descending' (case insensitive)");
            RuleFor(x => x).Must(x => x.OrderBy != null || x.OrderState == null).WithMessage("Column must be specified to sort in ascending or descending order");
        }
        private bool ValidDateFormat(string date)
        {
            return DateTime.TryParseExact(date, "dd-MM-yyyy HH:mm",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
        

    }
}
