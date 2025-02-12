using Exam1.Models;
using Exam1.Models.GET;
using Exam1.Models.POST;
using Exam1.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam1.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly BookedTicketService _service;
        public BookedTicketController(BookedTicketService service)
        {
            _service = service;
        }

        [HttpGet("get-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Get(string BookedTicketId)
        {
            var data = await _service.Get(BookedTicketId);
            return Ok(data);
        }

        [HttpPost("book-ticket")]
        public async Task<IActionResult> Post([FromBody] List<BookingModel> request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            string validation = await _service.validateBookingList(request);
            if(!validation.Equals("Ok"))
            {
                return BadRequest("Invalid Data: "+ validation);
            }
            var data = await _service.Post(request);
            return Ok(data);
        }
        [HttpDelete("revoke-ticket/{BookedTicketId}/{TicketCode}/{Quantity}")]
        public async Task<IActionResult> Delete(string BookedTicketId, string TicketCode, int Quantity)
        {
            string validation = await _service.validateDeletion(BookedTicketId, TicketCode, Quantity);
            if (!validation.Equals("Ok"))
            {
                return BadRequest("Invalid Data: " + validation);
            }
            var data = await _service.Delete(BookedTicketId, TicketCode, Quantity);
            return Ok(data);
        }
        [HttpDelete("revoke-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Delete(string BookedTicketId, [FromBody] List<BookingModel> request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            string validation = await _service.validateBatchDeletion(BookedTicketId, request);
            if (!validation.Equals("Ok"))
            {
                return BadRequest("Invalid Data: " + validation);
            }
            var data = await _service.BatchDelete(BookedTicketId, request);
            return Ok(data);
        }
        [HttpPut("edit-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Put(string BookedTicketId, [FromBody] List<BookingModel> request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            string validation = await _service.validateBatchEdit(BookedTicketId, request);
            if (!validation.Equals("Ok"))
            {
                return BadRequest("Invalid Data: " + validation);
            }
            var data = await _service.Put(BookedTicketId, request);
            return Ok(data);

        }


        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
