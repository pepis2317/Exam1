using Exam1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text.Json;
using MediatR;
using Exam1.Models;
using FluentValidation;
using Exam1.Models.Cart;

namespace Exam1.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BookedTicketController> _logger;
        private readonly IValidator<CheckTicketRequest> _ticketValidator;
        private readonly IValidator<CompleteTransactionRequest> _completeTransactionValidator;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        public CartController(IMediator mediator, ILogger<BookedTicketController> logger, IValidator<CheckTicketRequest> ticketValidator, IValidator<CompleteTransactionRequest> completeTransactionValidator)
        {
            _mediator = mediator;
            _logger = logger;
            _ticketValidator = ticketValidator;
            _completeTransactionValidator = completeTransactionValidator;
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
        [HttpGet("get-cart-incomplete/{UserId}")]
        public async Task<IActionResult> GetCart(Guid UserId)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var model = new UserIdCart
            {
                UserId = UserId,
                HandlerAction = "incomplete"
            };
            
            var data = await _mediator.Send(model);
            _logger.LogInformation("Fetched data for UserId: {@UserId}, Response: {@Data}", UserId, data);
            return new JsonResult(data, _jsonOptions);
        }
        [HttpGet("get-cart-completed/{UserId}")]
        public async Task<IActionResult> GetTransactionHistory(Guid UserId)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var model = new UserIdCart
            {
                UserId = UserId,
                HandlerAction = "completed"
            };
            var data = await _mediator.Send(model);
            _logger.LogInformation("Fetched data for UserId: {@UserId}, Response: {@Data}", UserId, data);
            return new JsonResult(data, _jsonOptions);
        }
        [HttpGet("get-ticket-in-cart/{UserId}/{TicketCode}")]
        public async Task<IActionResult> CheckTicket(Guid UserId, string TicketCode)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
           
            var model = new CheckTicketRequest
            {
                UserId = UserId,
                TicketCode = TicketCode
            };
            var validation = await _ticketValidator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var check = await _mediator.Send(model);
            _logger.LogInformation("Fetched data for UserId: {@UserId}, TicketCode:{@TicketCode} Response: {@Data}", UserId, TicketCode, check);
            return new JsonResult(check, _jsonOptions);
        }
        [HttpPut("finish-transaction/{UserId}/{BookingId}")]
        public async Task<IActionResult> finishTransaction(Guid UserId, string BookingId)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var model = new CompleteTransactionRequest
            {
                UserId = UserId,
                BookingId = BookingId
            };
            var validation = await _completeTransactionValidator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(model);
            _logger.LogInformation("Fetched data for UserId: {@UserId} BookingId: {@BookingId}, Response: {@Data}", UserId, BookingId, data);
            return new JsonResult(data, _jsonOptions);
        }
    }
}
