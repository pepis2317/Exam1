using Azure.Core;
using Exam1.Entities;
using Exam1.Models;
using Exam1.Models.GET;
using Exam1.Models.POST;
using Exam1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam1.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly ILogger<BookedTicketController> _logger;
        private readonly BookedTicketService _service;
        public BookedTicketController(BookedTicketService service, ILogger<BookedTicketController> logger)
        {
            _service = service;
            _logger = logger;  
        }

        [HttpGet("get-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Get(string BookedTicketId)
        {
            var data = await _service.Get(BookedTicketId);
            if(data.IsNullOrEmpty())
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = "Invalid booking id",
                    Instance = HttpContext.Request.Path
                };
                _logger.LogWarning("Invalid request data: {@ProblemDetails}", problemDetails);
                return BadRequest(problemDetails);
            }
            _logger.LogInformation("Fetched data for BookedTicketId: {@BookedTicketId}, Response: {@Data}", BookedTicketId, data);
            return Ok(data);
        }

        [HttpPost("book-ticket")]
        public async Task<IActionResult> Post([FromBody] List<BookingModel> request)
        {
            if(!ModelState.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Type = "http://veryCoolAPI.com/errors/validation-error",
                    Title = "Invalid Request Parameters",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Invalid modelState",
                    Instance = HttpContext.Request.Path
                };
                _logger.LogWarning("Validation error: {@ProblemDetails}", problemDetails);
                return BadRequest(problemDetails);
            }
            string validation = await _service.validateBookingList(request);
            if(!validation.Equals("Ok"))
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = validation,
                    Instance = HttpContext.Request.Path
                };
                _logger.LogWarning("Invalid request data: {@ProblemDetails}", problemDetails);
                return BadRequest(problemDetails);
            }
            var data = await _service.Post(request);
            _logger.LogInformation("Fetched data for request: {@Request}, Response: {@Data}", request, data);
            return Ok(data);
        }
        [HttpDelete("revoke-ticket/{BookedTicketId}/{TicketCode}/{Quantity}")]
        public async Task<IActionResult> Delete(string BookedTicketId, string TicketCode, int Quantity)
        {
            string validation = await _service.validateDeletion(BookedTicketId, TicketCode, Quantity);
            if (!validation.Equals("Ok"))
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = validation,
                    Instance = HttpContext.Request.Path
                };
                _logger.LogWarning("Invalid request data: {@ProblemDetails}", problemDetails);
                return BadRequest(problemDetails);
            }
            var data = await _service.Delete(BookedTicketId, TicketCode, Quantity);
            _logger.LogInformation("Fetched data for BookedTicketId: {@BookedTicketId}, TicketCode: {@TicketCode}, Quantity: {@TicketCode}, Response: {@Data}", BookedTicketId,TicketCode,Quantity, data);
            return Ok(data);
        }
        [HttpDelete("revoke-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Delete(string BookedTicketId, [FromBody] List<BookingModel> request)
        {
            if (!ModelState.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Type = "http://veryCoolAPI.com/errors/validation-error",
                    Title = "Invalid Request Parameters",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Invalid modelState",
                    Instance = HttpContext.Request.Path
                };
                _logger.LogWarning("Validation error: {@ProblemDetails}", problemDetails);
                return BadRequest(problemDetails);
            }
            string validation = await _service.validateBatchDeletion(BookedTicketId, request);
            if (!validation.Equals("Ok"))
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = validation,
                    Instance = HttpContext.Request.Path
                };
                _logger.LogWarning("Invalid request data: {@ProblemDetails}", problemDetails);
                return BadRequest(problemDetails);
            }
            var data = await _service.BatchDelete(BookedTicketId, request);
            _logger.LogInformation("Fetched data for BookedTicketId: {@BookedTicketId}, request: {@Request}, Response: {@Data}", BookedTicketId,request, data);
            return Ok(data);
        }
        [HttpPut("edit-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Put(string BookedTicketId, [FromBody] List<BookingModel> request)
        {
            if (!ModelState.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Type = "http://veryCoolAPI.com/errors/validation-error",
                    Title = "Invalid Request Parameters",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Invalid modelState",
                    Instance = HttpContext.Request.Path
                };
                _logger.LogWarning("Validation error: {@ProblemDetails}", problemDetails);
                return BadRequest(problemDetails);
            }
            string validation = await _service.validateBatchEdit(BookedTicketId, request);
            if (!validation.Equals("Ok"))
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = validation,
                    Instance = HttpContext.Request.Path
                };
                _logger.LogWarning("Invalid request data: {@ProblemDetails}", problemDetails);
                return BadRequest(problemDetails);
            }
            var data = await _service.Put(BookedTicketId, request);
            _logger.LogInformation("Fetched data for BookedTicketId: {@BookedTicketId}, request: {@Request}, Response: {@Data}", BookedTicketId, request, data);
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
