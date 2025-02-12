using Exam1.Models.GET;
using Exam1.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam1.Controllers
{
    [Route("api/v1/get-available-ticket")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        private readonly TicketService _service;
        public TicketController(TicketService service, ILogger<TicketController> logger)
        {
            _service = service;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TicketRequestModel request)
        {
            if (ModelState.IsValid == false)
            {
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Type ="http://veryCoolAPI.com/errors/validation-error",
                    Title = "Invalid Request Parameters",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "One or more validation error occured",
                    Instance = HttpContext.Request.Path
                };
                _logger.LogWarning("Validation error: {@ProblemDetails}", problemDetails);
                return BadRequest(problemDetails);
            }
            var validation = _service.validateQuery(request);
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
            var data = await _service.Get(request);
            _logger.LogInformation("Fetched data for request: {@Request}, Response: {@Data}", request, data);
            return Ok(data);
        }
    }
}
