
using Exam1.Services;
using Exam1.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text.Json;
using Exam1.Models.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam1.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IValidator<UserRequestModel> _userRequestValidator;
        private readonly IValidator<UserSignInModel> _userSignInValidator;
        private readonly IValidator<UserEditModel> _userEditValidator;
        private readonly IValidator<UserDeleteModel> _userDeleteValidator;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly IMediator _mediator;
        public UserController(
            IMediator mediator, 
            ILogger<UserController> logger, 
            IValidator<UserRequestModel> validator,
            IValidator<UserSignInModel> siginInValidator,
            IValidator<UserEditModel> userEditValidator,
            IValidator<UserDeleteModel> userDeleteValidator
            )
        {
            _logger = logger;
            _mediator = mediator;
            _userRequestValidator = validator;
            _userSignInValidator = siginInValidator;
            _userEditValidator = userEditValidator;
            _userDeleteValidator = userDeleteValidator;

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
        [HttpGet("get-all-users")]
        public async Task<IActionResult> Get()
        {
            var data = await _mediator.Send(new UsersRequestModel { });
            if(data.IsNullOrEmpty())
            {
                return Invalid("No users exist");
            }
            _logger.LogInformation("Fetched data with Response: {@Data}", data);
            return new JsonResult(data, _jsonOptions);
        }

        [HttpGet("get-user")]
        public async Task<IActionResult> Get([FromQuery]UserRequestModel request)
        {
            if(!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var validation = await _userRequestValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(request);
            _logger.LogInformation("Fetched data for request: {@Request}, Response: {@Data}", request, data);
            return Ok(data);
        }
        [HttpGet("get-user-by-id/{UserId}")]
        public async Task<IActionResult> Get(Guid UserId)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var getModel = new UserGetByIdModel 
            {
                UserId = UserId 
            };
            var data = await _mediator.Send(getModel);
            _logger.LogInformation("Fetched data for userId: {@UserId}, Response: {@Data}", UserId, data);
            return Ok(data);
        }

        // POST api/<UserController>
        [HttpPost("register-user")]
        public async Task<IActionResult> Post([FromBody] UserSignInModel request)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var validation = await _userSignInValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(request);
            _logger.LogInformation("Fetched data for request: {@Request}, Response: {@Data}", request, data);
            return Ok(data);
        }
        [HttpPut("edit-user")]
        public async Task<IActionResult> Put([FromBody] UserEditModel request)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var validation = await _userEditValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(request);
            _logger.LogInformation("Fetched data for request: {@Request}, Response: {@Data}", request, data);
            return Ok(data);
        }
        [HttpDelete("delete-user/{UserId}")]
        public async Task<IActionResult> Delete(Guid UserId)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var deleteModel = new UserDeleteModel
            {
                UserId = UserId
            };
            var validation = await _userDeleteValidator.ValidateAsync(deleteModel);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _mediator.Send(deleteModel);
            _logger.LogInformation("Fetched data for userId: {@UserId}, Response: {@Data}", UserId, data);
            return Ok(data);
        }
    }
}
