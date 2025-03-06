using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Exam1.Entities;
using Exam1.Models.Cart;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;

namespace Exam1.Services
{
    public class CartService
    {
        private readonly AccelokaContext _db;
        private readonly BookedTicketService _bookedTicketService;
        public CartService(AccelokaContext db, BookedTicketService bookedTicketService)
        {
            _db = db;
            _bookedTicketService = bookedTicketService;
        }
        
        public async Task<List<CartResponseModel>> GetIncompleteTransactions(Guid userId)
        {
            var cartData = await _db.Carts.Where(q => q.UserId == userId && q.IsCompleted=="false").OrderByDescending(q => q.BookedTicketId).FirstOrDefaultAsync();
            if (cartData != null)
            {
                var ticketsData = await _bookedTicketService.Get(cartData.BookedTicketId);
                return new List<CartResponseModel>
                {
                    new CartResponseModel
                    {
                        BookingId = cartData.BookedTicketId,
                        BookedTickets = ticketsData,
                        IsCompleted = cartData.IsCompleted
                    }
                };
            }
            return new List<CartResponseModel>
            {
                new CartResponseModel
                {
                    BookingId = "",
                    BookedTickets = [],
                    IsCompleted = ""
                }
            };
            


        }
        public async Task<List<CartResponseModel>> GetCompletedTransactions(Guid userId)
        {
            var bookedTicketIds = await _db.Carts.Where(q => q.UserId == userId && q.IsCompleted == "true").OrderByDescending(q=>q.BookedTicketId).Select(q => q.BookedTicketId).ToListAsync();
            List<CartResponseModel> result = new List<CartResponseModel>();
            foreach (var bookingId in bookedTicketIds)
            {
                var data = await _bookedTicketService.Get(bookingId);
                var isCompleted = await _db.Carts.Where(q => q.UserId == userId && q.BookedTicketId == bookingId).Select(q => q.IsCompleted).FirstOrDefaultAsync();
                result.Add(new CartResponseModel
                {
                    BookingId = bookingId,
                    BookedTickets = data,
                    IsCompleted = isCompleted
                });
            }
            return result;
        }
        public async Task<bool> checkCart(Guid userId, string ticketCode)
        {
            var cartData = await _db.Carts.Where(q => q.UserId == userId && q.IsCompleted == "false").OrderByDescending(q => q.BookedTicketId).FirstOrDefaultAsync();
            if (cartData != null)
            {
                var ticket = await _db.BookedTickets.Where(q => q.BookedTicketId == cartData.BookedTicketId && q.TicketCode == ticketCode).FirstOrDefaultAsync();
                if(ticket != null)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<CartResponseModel> CompleteTransaction(Guid userId, string bookingId)
        {
            _db.Carts.Where(q => q.UserId == userId && q.BookedTicketId == bookingId).ExecuteUpdate(q => q.SetProperty(c => c.IsCompleted, "true"));
            var ticketsData = await _bookedTicketService.Get(bookingId);
            return new CartResponseModel
            {
                BookingId = bookingId,
                BookedTickets = ticketsData,
                IsCompleted = "true"
            };

        }
    }
}
