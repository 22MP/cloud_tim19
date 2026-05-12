using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Devices.Commands.Registration
{
    public record RegisterDeviceCommand : IRequest<Result>
    {
        public string HiveId { get; init; } = string.Empty;
        public string SerialNumber { get; init; } = string.Empty;
    }
}
