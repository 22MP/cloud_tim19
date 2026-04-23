using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Users.Commands;
using SmartApiary.Application.Features.Users.Commands.ManageUser;
using SmartApiary.Application.Features.Users.Queries;
using SmartApiary.WebApi.Extensions;

namespace SmartApiary.WebApi.Controllers;

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
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetUsersQuery());
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id));
        return result.ToActionResult();
    }

    [HttpPatch("{id}/suspend")]
    public async Task<IActionResult> Suspend(string id)
    {
        var result = await mediator.Send(new SuspendUserCommand(id));
        return result.ToActionResult();
    }

    [HttpPatch("{id}/reactivate")]
    public async Task<IActionResult> Reactivate(string id)
    {
        var result = await mediator.Send(new ReactivateUserCommand(id));
        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await mediator.Send(new DeleteUserCommand(id));
        return result.ToActionResult();
    }
}