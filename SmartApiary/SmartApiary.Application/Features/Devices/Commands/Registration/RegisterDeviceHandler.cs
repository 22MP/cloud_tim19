using MediatR;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Devices.Commands.Registration
{
    internal class RegisterDeviceHandler : IRequestHandler<RegisterDeviceCommand, Result>
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHiveRepository _hiveRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RegisterDeviceHandler(IDeviceRepository deviceRepository, IHiveRepository hiveRepository, IDateTimeProvider dateTimeProvider)
        {
            _deviceRepository = deviceRepository;
            _hiveRepository = hiveRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
        {
            Result<EntityId> hiveIdResult = EntityId.Create(request.HiveId);
            
            if (hiveIdResult.IsFailure) return Result.Failure(hiveIdResult.Error!.Message, ErrorType.Validation);

            var hive = await _hiveRepository.GetByIdAsync(hiveIdResult.Value, cancellationToken);
            
            if (hive is null) return Result.Failure("Hive not found.", ErrorType.NotFound);

            var existing = await _deviceRepository.GetByHiveIdAsync(hiveIdResult.Value, cancellationToken);
            
            if (existing is not null) return Result.Failure("This hive already has a device.", ErrorType.Conflict);

            var bySn = await _deviceRepository.GetBySerialNumberAsync(request.SerialNumber, cancellationToken);
            
            if (bySn is not null) return Result.Failure("Serial number already registered.", ErrorType.Conflict);

            var deviceResult = Device.Create(request.HiveId, request.SerialNumber, _dateTimeProvider.UtcNow);

            if (deviceResult.IsFailure) return Result.Failure(deviceResult.Error!.Message, ErrorType.Validation);

            await _deviceRepository.AddAsync(deviceResult.Value, cancellationToken);
            
            return Result.Success();
        }
    }
}
