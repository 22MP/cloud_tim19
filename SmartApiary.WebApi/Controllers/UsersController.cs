using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Users.Commands;
using SmartApiary.Application.Features.Users.Queries;
using SmartApiary.WebApi.Extensions;

namespace SmartApiary.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            
            var result = await mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            
            var result = await mediator.Send(new GetUsersQuery());
            return result.ToActionResult();
        }
    }
}