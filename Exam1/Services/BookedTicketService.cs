using Exam1.Entities;
using Exam1.Models;
using Exam1.Models.GET;
using Exam1.Models.POST;
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
        private string GenerateNewId()
        {
            var latestId = _db.BookedTickets.OrderByDescending(q => q.BookedTicketId).Select(q => q.BookedTicketId).FirstOrDefault();
            if(latestId != null)
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
        public async Task<string> validateBookingList(List<BookingModel> bookingList)
        {
            foreach(BookingModel booking in bookingList)
            {
                if(booking.Quantity <= 0)
                {
                    return "Booking quantity must not be 0";
                }
                else
                {
                    var ticket = await _db.Tickets.Include(q => q.Category).Where(q => q.TicketCode == booking.TicketCode).FirstOrDefaultAsync();
                    if (ticket != null)
                    {
                        if(ticket.EventDate <= DateTime.Now)
                        {
                            return "Booking must be an event in the future";
                        }
                        if(ticket.Quota < booking.Quantity)
                        {
                            return "Booking must be <= the available quota";
                        }

                    }
                    else
                    {
                        return "Invalid ticket code";
                    }
                }
            }
            return "Ok";
        }

        public async Task<BookTicketResponseModel?> Post(List<BookingModel> bookingList)
        {
            int priceSummary = 0;
            Dictionary<string, TicketsPerCategoryModel> categories = new Dictionary<string, TicketsPerCategoryModel> ();
            string id = GenerateNewId();
            foreach(BookingModel booking in bookingList)
            {
                var ticket= await _db.Tickets.Include(q=>q.Category).Where(q => q.TicketCode == booking.TicketCode).FirstOrDefaultAsync();
                if (ticket!= null)
                {
                    ticket.Quota -= booking.Quantity;
                    priceSummary += ticket.Price * booking.Quantity;
                    _db.Tickets.Update(ticket);

                    string categoryName = ticket.Category.CategoryName;
                    if (categories.ContainsKey(categoryName))
                    {
                        var category = categories[ticket.Category.CategoryName];
                        category.Tickets.Add(new TicketInfo
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
                            Tickets = new List<TicketInfo>() 
                            { 
                                new TicketInfo 
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
                    BookedTicketId = id,
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
        public async Task<List<BookedTicketPerCategoryModel>> Get(string id)
        {
            Dictionary<string, BookedTicketPerCategoryModel> categories = new Dictionary<string, BookedTicketPerCategoryModel>();
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
                        category.Tickets.Add(new BookedTicketModel
                        {
                            TicketCode = ticket.TicketCode,
                            TicketName = ticket.TicketName,
                            EventDate = ticket.EventDate.ToString("dd-MM-yyyy hh:mm"),
                        });
                        category.QtyPerCategory += booking.Quantity;
                    }
                    else
                    {
                        categories.Add(categoryName, new BookedTicketPerCategoryModel
                        {
                            QtyPerCategory = booking.Quantity,
                            CategoryName = categoryName,
                            Tickets = new List<BookedTicketModel>()
                            {
                                new BookedTicketModel
                                {
                                    TicketCode = ticket.TicketCode,
                                    TicketName = ticket.TicketName,
                                    EventDate = ticket.EventDate.ToString("dd-MM-yyyy hh:mm")
                                }
                            }
                        });
                    }
                }
            }
            List<BookedTicketPerCategoryModel> data = new List<BookedTicketPerCategoryModel>();
            foreach(var category in categories)
            {
                data.Add(category.Value);
            }
            return data;
        }
        public async Task<string> validateDeletion(string BookedTicketId, string TicketCode, int Quantity)
        {
            var bookingData = await _db.BookedTickets.Where(q => q.BookedTicketId == BookedTicketId).Where(q => q.TicketCode == TicketCode).FirstOrDefaultAsync();
            if (bookingData == null)
            {
                return "Invalid booking id/ticket code";
            }
            if (Quantity > bookingData.Quantity)
            {
                return "Quantity deleted must be less than or equal to booked quantity";
            }
            return "Ok";
        }
        public async Task<string> validateBatchDeletion(string BookedTicketId, List<BookingModel> request)
        {
            HashSet<string> ticketCodes = new HashSet<string>();
            foreach (var item in request)
            {
                if (ticketCodes.Contains(item.TicketCode))
                {
                    return "No duplicates allowed";
                }
                else
                {
                    var bookingData = await _db.BookedTickets.Where(q => q.BookedTicketId == BookedTicketId).Where(q => q.TicketCode == item.TicketCode).FirstOrDefaultAsync();
                    if (bookingData == null)
                    {
                        return "Invalid booking id/ticket code";
                    }
                    if (item.Quantity > bookingData.Quantity)
                    {
                        return "Quantity deleted must be less than or equal to booked quantity";
                    }
                    ticketCodes.Add(item.TicketCode);
                }
            }
            return ("Ok");
        }
        public async Task<string> validateBatchEdit(string BookedTicketId, List<BookingModel> request)
        {
            HashSet<string> ticketCodes = new HashSet<string>();
            foreach (var item in request)
            {
                if (ticketCodes.Contains(item.TicketCode))
                {
                    return "No duplicates allowed";
                }
                else
                {
                    var bookingData = await _db.BookedTickets.Where(q => q.BookedTicketId == BookedTicketId).Where(q => q.TicketCode == item.TicketCode).FirstOrDefaultAsync();
                    if (bookingData == null)
                    {
                        return "Invalid booking id/ticket code";
                    }
                    var ticketData = await _db.Tickets.Where(q => q.TicketCode == bookingData.TicketCode).FirstOrDefaultAsync();
                    if (ticketData == null)
                    {
                        return "Invalid ticket code";
                    }
                    if (item.Quantity > ticketData.Quota)
                    {
                        return "Quantity edited must be less than or equal to ticket quota";
                    }
                    if(item.Quantity < 1)
                    {
                        return "Quantity must be greater than equal to 1";
                    }

                    ticketCodes.Add(item.TicketCode);
                }
            }
            return ("Ok");
        }

        public async Task<List<ChangedTicketModel>> Delete(string bookedTicketId, string ticketCode, int quantity)
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
        public async Task<List<ChangedTicketModel>> BatchDelete(string bookedTicketId, List<BookingModel> request)
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
        public async Task<List<ChangedTicketModel>> Put(string bookedTicketId, List<BookingModel> request)
        {
            foreach (var edit in request)
            {
                var bookingData = await _db.BookedTickets.Where(q => q.BookedTicketId == bookedTicketId).Where(q => q.TicketCode == edit.TicketCode).FirstOrDefaultAsync();
                if (bookingData != null)
                {
                    if (edit.Quantity == bookingData.Quantity)
                    {
                        _db.BookedTickets.Remove(bookingData);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        bookingData.Quantity = edit.Quantity;
                        _db.BookedTickets.Update(bookingData);
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return await GetChangedTickets(bookedTicketId);
        }

        private async Task<List<ChangedTicketModel>> GetChangedTickets(string bookedTicketId)
        {
            List<ChangedTicketModel> changedTickets = new List<ChangedTicketModel>();
            var bookedTickets = await _db.BookedTickets.Where(q=>q.BookedTicketId==bookedTicketId).ToListAsync();
            foreach(var bookedTicket in bookedTickets)
            {
                var ticket = await _db.Tickets.Include(q=>q.Category).Where(q=>q.TicketCode == bookedTicket.TicketCode).FirstOrDefaultAsync();
                if (ticket != null)
                {
                    changedTickets.Add(new ChangedTicketModel
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
