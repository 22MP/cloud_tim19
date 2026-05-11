using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using SmartApiary.Application.Features.Devices.Commands.Handshake;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartApiary.Functions.Extensions;

namespace SmartApiary.Functions.Ingestion
{
    internal class HandshakeFunction
    {
        private readonly IMediator _mediator;

        public HandshakeFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("DeviceHandshake")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post",Route = "device/handshake")] HttpRequest req)
        {
            HandshakeCommand? command = await req.ReadFromJsonAsync<HandshakeCommand>();

            if (command is null) return new BadRequestObjectResult(new
            {
                error = "Validation",
                details = "Invalid request body."
            });

            Result<HandshakeResponse> result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
