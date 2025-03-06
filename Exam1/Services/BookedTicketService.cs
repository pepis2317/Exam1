using Azure.Core;
using Exam1.Entities;
using Exam1.Models.Booking;
using Exam1.Models.Ticket;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics;

namespace Exam1.Services
{
    public class BookedTicketService
    {
        private readonly AccelokaContext _db;
        public BookedTicketService(AccelokaContext db)
        {
            _db = db;
        }
        
        private async Task<string> GenerateNewId()
        {
            var latestId = await _db.Carts.OrderByDescending(q => q.BookedTicketId).Select(q => q.BookedTicketId).FirstOrDefaultAsync();
            if (latestId != null)
            {
                string numericPart = latestId.Substring(1);
                if (int.TryParse(numericPart, out int number))
                {
                    number++;
                    return $"B{number:D3}";
                }
            }
            return "B001";
        }

        public async Task<BookTicketResponseModel> Post(Guid userId, List<BookingModel> bookingList)
        {
            var bookedTicketId = "";
            var cartData = await _db.Carts.Where(q => q.UserId == userId).OrderByDescending(q => q.BookedTicketId).FirstOrDefaultAsync();
            if (cartData!=null)
            {
                if(cartData.IsCompleted == "false")
                {
                    bookedTicketId = cartData.BookedTicketId;
                }
                else
                {
                    bookedTicketId = await GenerateNewId();
                    _db.Carts.Add(new Cart
                    {
                        CartId = Guid.NewGuid(),
                        UserId = userId,
                        BookedTicketId = bookedTicketId,
                        IsCompleted = "false"
                    });
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                bookedTicketId = await GenerateNewId();
                _db.Carts.Add(new Cart
                {
                    CartId = Guid.NewGuid(),
                    UserId = userId,
                    BookedTicketId = bookedTicketId,
                    IsCompleted = "false"
                });
                await _db.SaveChangesAsync();
            }
           

            int priceSummary = 0;
            Dictionary<string, TicketsPerCategoryModel> categories = new Dictionary<string, TicketsPerCategoryModel> ();
            foreach(BookingModel booking in bookingList)
            {
                var ticket= await _db.Tickets.Include(q=>q.Category).Where(q => q.TicketCode == booking.TicketCode).FirstOrDefaultAsync();
                if (ticket!= null)
                {
                    priceSummary += ticket.Price * booking.Quantity;


                    string categoryName = ticket.Category.CategoryName;
                    if (categories.ContainsKey(categoryName))
                    {
                        var category = categories[ticket.Category.CategoryName];
                        category.Tickets.Add(new TicketInfoModel
                        {
                            TicketCode = ticket.TicketCode,
                            TicketName = ticket.TicketName,
                            Price = ticket.Price
                        });
                        category.SummaryPrice += ticket.Price * booking.Quantity;
                    }
                    else
                    {
                        categories.Add(categoryName, new TicketsPerCategoryModel
                        {
                            CategoryName = categoryName,
                            SummaryPrice = ticket.Price * booking.Quantity,
                            Tickets = new List<TicketInfoModel>() 
                            { 
                                new TicketInfoModel 
                                {
                                TicketCode = ticket.TicketCode,
                                TicketName = ticket.TicketName,
                                Price = ticket.Price
                                } 
                            }
                        });
                    }
                }
                
                var bookingData = new BookedTicket
                {
                    Id = Guid.NewGuid(),
                    BookedTicketId = bookedTicketId,
                    TicketCode = booking.TicketCode,
                    Quantity = booking.Quantity
                };
                _db.BookedTickets.Add(bookingData);
            }
            await _db.SaveChangesAsync();

            List<TicketsPerCategoryModel> ticketsPerCategories = new List<TicketsPerCategoryModel>();
            foreach (var ticketCategory in categories)
            {
                ticketsPerCategories.Add(ticketCategory.Value);
            }
            var data = new BookTicketResponseModel
            {
                PriceSummary = priceSummary,
                TicketsPerCategory = ticketsPerCategories
            };
            return data;
            
        }
        public async Task<List<TicketsPerCategoryModel>> Get(string id)
        {
            Dictionary<string, TicketsPerCategoryModel> categories = new Dictionary<string, TicketsPerCategoryModel>();
            var bookedTickets = await _db.BookedTickets.Where(q=>q.BookedTicketId == id).ToListAsync();
            foreach (var booking in bookedTickets)
            {
                var ticket = await _db.Tickets.Include(q => q.Category).Where(q => q.TicketCode == booking.TicketCode).FirstOrDefaultAsync();
                if(ticket != null)
                {
                    string categoryName = ticket.Category.CategoryName;
                    if (categories.ContainsKey(categoryName))
                    {
                        var category = categories[ticket.Category.CategoryName];
                        category.Tickets.Add(new TicketInfoModel
                        {
                            TicketCode = ticket.TicketCode,
                            TicketName = ticket.TicketName,
                            EventDate = ticket.EventDate.ToString("dd-MM-yyyy hh:mm"),
                            Quantity = booking.Quantity,
                            TotalPrice = ticket.Price * booking.Quantity
                        });
                        category.QtyPerCategory += booking.Quantity;
                        category.SummaryPrice += ticket.Price * booking.Quantity;
                    }
                    else
                    {
                        categories.Add(categoryName, new TicketsPerCategoryModel
                        {
                            QtyPerCategory = booking.Quantity,
                            CategoryName = categoryName,
                            Tickets = new List<TicketInfoModel>()
                            {
                                new TicketInfoModel
                                {
                                    TicketCode = ticket.TicketCode,
                                    TicketName = ticket.TicketName,
                                    EventDate = ticket.EventDate.ToString("dd-MM-yyyy hh:mm"),
                                    Quantity = booking.Quantity,
                                    TotalPrice = ticket.Price * booking.Quantity
                                }
                            },
                            SummaryPrice = ticket.Price * booking.Quantity

                        });
                    }
                }
            }
            List<TicketsPerCategoryModel> data = new List<TicketsPerCategoryModel>();
            foreach(var category in categories)
            {
                data.Add(category.Value);
            }
            return data;
        }

        public async Task<List<TicketInfoModel>> Delete(string bookedTicketId, string ticketCode, int quantity)
        {
            var bookingData = await _db.BookedTickets.Where(q => q.BookedTicketId == bookedTicketId).Where(q => q.TicketCode == ticketCode).FirstOrDefaultAsync();
            if (bookingData != null)
            {
                if (quantity == bookingData.Quantity)
                {
                    _db.BookedTickets.Remove(bookingData);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    bookingData.Quantity -= quantity;
                    _db.BookedTickets.Update(bookingData);
                    await _db.SaveChangesAsync();
                }
            }
            return await GetChangedTickets(bookedTicketId);
        }
        public async Task<List<TicketInfoModel>> BatchDelete(string bookedTicketId, List<BookingModel> request)
        {
            foreach(var revoke in request)
            {
                var bookingData = await _db.BookedTickets.Where(q => q.BookedTicketId == bookedTicketId).Where(q => q.TicketCode == revoke.TicketCode).FirstOrDefaultAsync();
                if (bookingData != null)
                {
                    if (revoke.Quantity == bookingData.Quantity)
                    {
                        _db.BookedTickets.Remove(bookingData);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        bookingData.Quantity -= revoke.Quantity;
                        _db.BookedTickets.Update(bookingData);
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return await GetChangedTickets(bookedTicketId);
        }
        public async Task<List<TicketInfoModel>> Put(string bookedTicketId, List<BookingModel> request)
        {
            foreach (var edit in request)
            {
                var bookingData = await _db.BookedTickets.Where(q => q.BookedTicketId == bookedTicketId).Where(q => q.TicketCode == edit.TicketCode).FirstOrDefaultAsync();
                if (bookingData != null)
                {
                    var ticketData = await _db.Tickets.Where(q => q.TicketCode == edit.TicketCode).FirstOrDefaultAsync();
                    if (ticketData != null)
                    {
                        bookingData.Quantity = edit.Quantity;
                        _db.BookedTickets.Update(bookingData);
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return await GetChangedTickets(bookedTicketId);
        }

        private async Task<List<TicketInfoModel>> GetChangedTickets(string bookedTicketId)
        {
            List<TicketInfoModel> changedTickets = new List<TicketInfoModel>();
            var bookedTickets = await _db.BookedTickets.Where(q=>q.BookedTicketId==bookedTicketId).ToListAsync();
            foreach(var bookedTicket in bookedTickets)
            {
                var ticket = await _db.Tickets.Include(q=>q.Category).Where(q=>q.TicketCode == bookedTicket.TicketCode).FirstOrDefaultAsync();
                if (ticket != null)
                {
                    changedTickets.Add(new TicketInfoModel
                    {
                        TicketCode = ticket.TicketCode,
                        TicketName = ticket.TicketName,
                        Quantity = bookedTicket.Quantity,
                        CategoryName = ticket.Category.CategoryName
                    });
                }
                
            }
            return changedTickets;
        }

    }
}
