using Exam1.Entities;
using Exam1.Models.GET;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Exam1.Services
{
    public class TicketService
    {
        private readonly AccelokaContext _db;
        public TicketService(AccelokaContext db)
        {
            _db = db;
        }
        public async Task<TicketsResponseModel> Get(TicketRequestModel request)
        {
            var query = _db.Tickets.AsQueryable();
            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                query = query.Where(q => q.Category.CategoryName.ToLower().Contains( request.CategoryName.ToLower()));
            }
            if(!string.IsNullOrEmpty(request.TicketCode))
            {
                query = query.Where(q => q.TicketCode.ToLower().Contains(request.TicketCode.ToLower()));
            }
            if(!string.IsNullOrEmpty(request.TicketName))
            {
                query = query.Where(q => q.TicketName.ToLower().Contains(request.TicketName.ToLower()));
            }
            if (!string.IsNullOrEmpty(request.MinDate))
            {
                var minDate = DateTime.ParseExact(request.MinDate, "dd-MM-yyyy hh:mm", CultureInfo.InvariantCulture);
                query = query.Where(q => q.EventDate >= minDate);
            }
            if (!string.IsNullOrEmpty(request.MaxDate))
            {
                var maxDate = DateTime.ParseExact(request.MaxDate, "dd-MM-yyyy hh:mm", CultureInfo.InvariantCulture);
                query = query.Where(q => q.EventDate <= maxDate);
            }
            if (request.Price !=0)
            {
                query = query.Where(q => q.Price <= request.Price);
            }
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                Dictionary<string, string> validColumns = new Dictionary<string, string>()
                {
                    {"categoryname", "CategoryName" },
                    {"ticketcode", "TicketCode"},
                    {"ticketname", "TicketName"},
                    {"eventdate", "EventDate"},
                    {"price","Price"},
                    {"quota","Quota"}

                };

                if (validColumns.ContainsKey(request.OrderBy.ToLower()))
                {
                    string columnName = validColumns[request.OrderBy.ToLower()];
                    if (request.OrderState?.ToLower().Equals("descending", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        query = query.OrderByDescending(e => EF.Property<object>(e, columnName));
                    }
                    else
                    {
                        query = query.OrderBy(e => EF.Property<object>(e, columnName));
                    }
                }
            }


            var ticketsList = await query.Select(q => new TicketModel
            {
                EventDate = q.EventDate.ToString("dd-MM-yyyy hh:mm"), 
                Quota = q.Quota,
                TicketCode = q.TicketCode,
                TicketName = q.TicketName,
                CategoryName = q.Category.CategoryName,
                Price = q.Price
            }).Where(q => q.Quota > 0).ToListAsync();

            var totalTickets = 0;
            foreach (TicketModel ticket in ticketsList)
            {
                totalTickets += ticket.Quota;
            }
            var data = new TicketsResponseModel
            {
                Tickets = ticketsList,
                TotalTickets = totalTickets
            };
            return data;
        }
    }
}
