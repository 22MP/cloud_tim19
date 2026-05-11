using MediatR;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Enums.Devices;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Devices.Commands.Handshake
{
    internal class HandshakeHandler : IRequestHandler<HandshakeCommand, Result<HandshakeResponse>>
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public HandshakeHandler(IDeviceRepository deviceRepository, IDateTimeProvider dateTimeProvider)
        {
            _deviceRepository = deviceRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<HandshakeResponse>> Handle(HandshakeCommand command, CancellationToken cancellationToken)
        {
            Device? device = await _deviceRepository.GetBySerialNumberAsync(command.SerialNumber, cancellationToken);
            
            if (device is null) return Result<HandshakeResponse>.Failure("Device not found.", ErrorType.NotFound);

            if (device.Status == DeviceStatus.Paired) return Result<HandshakeResponse>.Failure("Device is already paired.", ErrorType.Conflict);

            string accessToken = GenerateAccessToken();

            Result result = device.CompleteHandshake(command.DeviceUuid, accessToken, _dateTimeProvider.UtcNow);

            if (result.IsFailure) return Result<HandshakeResponse>.Failure(result.Error!.Message, ErrorType.Validation);

            await _deviceRepository.UpdateAsync(device, cancellationToken);

            return Result<HandshakeResponse>.Success( new HandshakeResponse(accessToken));
        }

        private static string GenerateAccessToken()
        {
            byte[] bytes = new byte[32];
            using RandomNumberGenerator randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

    }
}
