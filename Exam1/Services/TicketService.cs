using Exam1.Entities;
using Exam1.Models.GET;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public string validateQuery(TicketRequestModel request)
        {
            
            if (request.MinDate != null)
            {
                DateTime dateValue;
                if (!DateTime.TryParseExact(request.MinDate, "dd-MM-yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                {
                    return "Min date is not in the correct format (dd-MM-yyyy HH:mm)";
                }
            }

            if (request.MaxDate != null)
            {
                DateTime dateValue;
                if (!DateTime.TryParseExact(request.MaxDate, "dd-MM-yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                {
                    return "Max date is not in the correct format (dd-MM-yyyy HH:mm)";
                }
            }
            if (request.OrderBy != null)
            {
                HashSet<string> validColumns = new HashSet<string>()
                {
                    "CategoryName","TicketCode","TicketName", "EventDate", "Price", "Quota"
                };
                if (!validColumns.Contains(request.OrderBy))
                {
                    return request.OrderBy + " is not a valid column name (column names are case sensitive)";
                }
                if (request.OrderState != null)
                {
                    if (!request.OrderState.ToLower().Equals("descending") && !request.OrderState.ToLower().Equals("ascending"))
                    {
                        return "Order state must be ascending or descending (case insensitive)";
                    }
                }

            }
            if(request.OrderBy == null && request.OrderState != null)
            {
                return "Column must be specified to sort in ascending or descending order";
            }
            return "Ok";
        }
        public async Task<TicketsResponseModel> Get(TicketRequestModel request)
        {
            var query = _db.Tickets.Include(q => q.Category).AsQueryable();
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
            if (request.Price != null)
            {
                query = query.Where(q => q.Price <= request.Price);
            }
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                bool isDescending = request.OrderState?.ToLower().Equals("descending") ?? false;
                if (request.OrderBy.Equals("CategoryName"))
                {
                    query = isDescending ? query.OrderByDescending(q => q.Category.CategoryName) : query.OrderBy(q => q.Category.CategoryName);
                }
                else
                {
                    query = isDescending ? query.OrderByDescending(q => EF.Property<object>(q, request.OrderBy)) : query.OrderBy(q => EF.Property<object>(q, request.OrderBy));
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
