using DocumentFormat.OpenXml.Spreadsheet;
using Exam1.Entities;
using Exam1.Models.Cart;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Validators
{
    public class CheckTicketValidator:AbstractValidator<CheckTicketRequest>
    {
        private readonly AccelokaContext _db;
        public CheckTicketValidator(AccelokaContext db)
        {
            _db = db;
            RuleFor(x=> x.TicketCode).MustAsync(ValidTicketCode).WithMessage("Invalid ticket code");
        }
        private async Task<bool> ValidTicketCode(string ticketCode, CancellationToken token)
        {
            var data = await _db.Tickets.FirstOrDefaultAsync(x=>x.TicketCode == ticketCode);
            if(data == null)
            {
                return false;
            }
            return true;
        }
    }
}
