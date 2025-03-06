using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text.Json;
using MediatR;
using Exam1.Validators.Booking;
using Exam1.Models.Booking;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam1.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly ILogger<BookedTicketController> _logger;
        private readonly IValidatorForBookingListPost _postValidator;
        private readonly IValidatorForBookingSingleDelete _deleteValidator;
        private readonly IValidatorForBookingListEdit _editValidator;
        private readonly IValidatorForBookingBatchDelete _batchDeleteValidator;
        private readonly IMediator _mediator;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        public BookedTicketController(
            IMediator mediator, 
            ILogger<BookedTicketController> logger, 
            IValidatorForBookingListPost postValidator, 
            IValidatorForBookingSingleDelete deleteValidator, 
            IValidatorForBookingListEdit editValidator, 
            IValidatorForBookingBatchDelete batchDeleteValidator
            )
        {
            _mediator = mediator;
            _logger = logger;  
            _postValidator = postValidator;
            _deleteValidator = deleteValidator;
            _editValidator = editValidator;
            _batchDeleteValidator = batchDeleteValidator;
        }
        private BadRequestObjectResult Invalid(string detail)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Invalid Request Data",
                Detail = detail,
                Instance = HttpContext.Request.Path
            };
            _logger.LogWarning("Invalid request data: {@ProblemDetails}", problemDetails);
            return BadRequest(problemDetails);
        }
        private BadRequestObjectResult InvalidModelState()
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

        [HttpGet("get-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Get(string BookedTicketId)
        {
            var request = new GetBookingRequest
            {
                BookedTicketId = BookedTicketId
            };
            var data = await _mediator.Send(request);
            if(data.IsNullOrEmpty())
            {
                return Invalid("Invalid booking id");
            }
            _logger.LogInformation("Fetched data for BookedTicketId: {@BookedTicketId}, Response: {@Data}", BookedTicketId, data);
            return new JsonResult(data, _jsonOptions);
        }
       

        [HttpPost("book-ticket/{UserId}")]
        public async Task<IActionResult> Post(Guid UserId,[FromBody] List<BookingModel> request)
        {
            if(!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var model = new UserIdBookingList
            {
                UserId = UserId,
                BookingList = request
            };
            var validation = await _postValidator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(model);
            _logger.LogInformation("Fetched data for request: {@Request}, Response: {@Data}", request, data);
            return new JsonResult(data, _jsonOptions);
        }
        [HttpDelete("revoke-ticket/{BookedTicketId}/{TicketCode}/{Quantity}")]
        public async Task<IActionResult> Delete(string BookedTicketId, string TicketCode, int Quantity)
        {
            var model = new BookingModel
            {
                BookedTicketId = BookedTicketId,
                TicketCode = TicketCode,
                Quantity = Quantity
            };
            var validation = await _deleteValidator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(model);
            _logger.LogInformation("Fetched data for BookedTicketId: {@BookedTicketId}, TicketCode: {@TicketCode}, Quantity: {@TicketCode}, Response: {@Data}", BookedTicketId,TicketCode,Quantity, data);
            return new JsonResult(data, _jsonOptions);
        }
        [HttpDelete("revoke-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Delete(string BookedTicketId, [FromBody] List<BookingModel> request)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var model = new BookingIdBookingListModel(BookedTicketId, request);
            model.HandlerAction = "delete";
            var validation = await _batchDeleteValidator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(model);
            _logger.LogInformation("Fetched data for BookedTicketId: {@BookedTicketId}, request: {@Request}, Response: {@Data}", BookedTicketId,request, data);
            return new JsonResult(data, _jsonOptions);
        }
        [HttpPut("edit-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Put(string BookedTicketId, [FromBody] List<BookingModel> request)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var model = new BookingIdBookingListModel(BookedTicketId, request);
            model.HandlerAction = "edit";
            var validation = await _editValidator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(model);
            _logger.LogInformation("Fetched data for BookedTicketId: {@BookedTicketId}, request: {@Request}, Response: {@Data}", BookedTicketId, request, data);
            return new JsonResult(data, _jsonOptions);

        }
    }
}
