using Exam1.Services;
using Exam1.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using MediatR;
using Exam1.Models.Ticket;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam1.Controllers
{
    [Route("api/v1/get-available-ticket")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        private readonly IValidator<TicketRequestModel> _validator;
        private readonly IMediator _mediator;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        public TicketController(IMediator mediator, ILogger<TicketController> logger, IValidator<TicketRequestModel> validator)
        {
            _mediator = mediator;
            _logger = logger;
            _validator = validator;
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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TicketRequestModel request)
        {
            if (ModelState.IsValid == false)
            {
                return InvalidModelState();
            }
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(request);
            _logger.LogInformation("Fetched data for request: {@Request}, Response: {@Data}", request, data);
            return new JsonResult(data, _jsonOptions);
        }
    }
}
