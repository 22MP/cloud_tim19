using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Users.Commands.ForgotPassword
{
    public record ForgotPasswordCommand : IRequest<Result>
    {
        public string Email { get; init; } = string.Empty;
    }
}
