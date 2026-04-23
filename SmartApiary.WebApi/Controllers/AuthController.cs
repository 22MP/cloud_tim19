using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Users.Commands.ActivateAccount;
using SmartApiary.Application.Features.Users.Commands.Login;
using SmartApiary.WebApi.Extensions;

namespace SmartApiary.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("activate")]
    public async Task<IActionResult> Activate([FromBody] ActivateAccountCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
}