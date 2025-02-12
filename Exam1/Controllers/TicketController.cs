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
        private readonly TicketService _service;
        public TicketController(TicketService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TicketRequestModel request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Invalid Data");
            }
            var data = await _service.Get(request);
            return Ok(data);
        }
    }
}
