using Exam1.Models.Category;
using Exam1.Models.User;
using Exam1.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Exam1.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator, ILogger<UserController> logger)
        {
            _logger = logger;
            _mediator = mediator;
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
        [HttpGet("get-all-categories")]
        public async Task<IActionResult> Get()
        {
            var data = await _mediator.Send(new CategoriesRequestModel { });
            if (data.IsNullOrEmpty())
            {
                return Invalid("No categories exist");
            }
            _logger.LogInformation("Fetched data with Response: {@Data}", data);
            return Ok(data);
        }
    }
}
