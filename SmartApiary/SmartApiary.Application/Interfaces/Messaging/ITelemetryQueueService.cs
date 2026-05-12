using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Messaging
{
    public interface ITelemetryQueueService
    {
        Task EnqueueAsync(TelemetrySample sample , CancellationToken ct = default);
    }
}
