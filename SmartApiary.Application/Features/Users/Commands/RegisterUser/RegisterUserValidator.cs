using FluentValidation;
using SmartApiary.Application.Common.Validators;
using SmartApiary.Domain.Enums.Users;

namespace SmartApiary.Application.Features.Users.Commands;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.Email).IsValidEmail();

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role.")
            .Must(r => r == UserRole.Beekeeper || r == UserRole.Farmer)
            .WithMessage("Admin role cannot be assigned this way.");
    }
}