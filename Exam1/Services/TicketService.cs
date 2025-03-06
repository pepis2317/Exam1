using Exam1.Entities;
using Exam1.Models.Ticket;
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


            var ticketsList = await query.Select(q => new TicketInfoModel
            {
                EventDate = q.EventDate.ToString("dd-MM-yyyy hh:mm"), 
                Quota = q.Quota,
                TicketCode = q.TicketCode,
                TicketName = q.TicketName,
                CategoryName = q.Category.CategoryName,
                Price = q.Price
            }).Where(q => q.Quota > 0).ToListAsync();

            var totalTickets = 0;
            foreach (TicketInfoModel ticket in ticketsList)
            {
                if(ticket.Quota != null)
                {
                    totalTickets += (int)ticket.Quota;
                }
                
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
