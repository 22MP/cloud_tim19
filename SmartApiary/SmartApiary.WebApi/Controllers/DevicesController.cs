using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Users.Commands.Login;

namespace SmartApiary.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public DevicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register()
        {
            // TODO: implement RegisterDeviceCommand
            return Ok(new { message = "Device registered (stub)" });
        }

        [HttpPost("telemetry")]
        public async Task<IActionResult> Telemetry()
        {
            // TODO: implement RegisterDeviceCommand
            return Ok(new { message = "Device registered (stub)" });
        }
    }
}
