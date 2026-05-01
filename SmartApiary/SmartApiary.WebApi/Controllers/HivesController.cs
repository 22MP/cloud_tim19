using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Hives.Commands.AddHive;
using SmartApiary.Application.Features.Hives.Commands.DeleteHive;
using SmartApiary.Application.Features.Hives.Commands.UpdateHive;
using SmartApiary.Application.Features.Hives.Queries.GetHiveById;
using SmartApiary.Application.Features.Hives.Queries.GetHivesByApiary;
using SmartApiary.Domain.Enums.Apiaries;
using SmartApiary.WebApi.DTOs;
using SmartApiary.WebApi.Extensions;

namespace SmartApiary.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Beekeeper")]
    public class HivesController(IMediator mediator) : ControllerBase
    {
        // GET api/hives?apiaryId={id}
        [HttpGet]
        public async Task<IActionResult> GetByApiary([FromQuery] string apiaryId, CancellationToken ct)
        {
            var result = await mediator.Send(new GetHivesByApiaryQuery(apiaryId), ct);
            return result.ToActionResult();
        }

        // GET api/hives/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var result = await mediator.Send(new GetHiveByIdQuery(id), ct);
            return result.ToActionResult();
        }

        // POST api/hives
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddHiveCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);
            return result.ToActionResult();
        }

        // PUT api/hives/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateHiveRequest request, CancellationToken ct)
        {
            var command = new UpdateHiveCommand
            {
                HiveId   = id,
                Label    = request.Label,
                Type     = request.Type,
                BoxColor = request.BoxColor,
                QueenAge = request.QueenAge,
                Note     = request.Note
            };
            var result = await mediator.Send(command, ct);
            return result.ToActionResult();
        }

        // DELETE api/hives/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var result = await mediator.Send(new DeleteHiveCommand(id), ct);
            return result.ToActionResult();
        }
    }
}
