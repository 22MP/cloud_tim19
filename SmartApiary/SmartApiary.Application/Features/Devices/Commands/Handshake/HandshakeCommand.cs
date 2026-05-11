using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Devices.Commands.Handshake
{
    public record HandshakeCommand : IRequest<Result<HandshakeResponse>>
    {
        public string DeviceUuid { get; init; } = string.Empty;
        public string SerialNumber { get; init; } = string.Empty;
    }

    public record HandshakeResponse(string AccessToken);
}
