using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Telemetry.Commands
{
    public record IngestTelemetryCommand : IRequest<Result>
    {
        public string AccessToken { get; init; } = string.Empty;
        public double WeightKg { get; init; }
        public double Humidity { get; init; }
        public double Temperature { get; init; }
        public int BatteryPercentage { get; init; }
        public DateTime RecordedAt { get; init; }
    }
}
