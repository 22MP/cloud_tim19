using FluentValidation;
using SmartApiary.Application.Common.Validators;

namespace SmartApiary.Application.Features.Users.Commands.ActivateAccount;

public class ActivateAccountValidator : AbstractValidator<ActivateAccountCommand>
{
    public ActivateAccountValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Activation token is required.");

        RuleFor(x => x.Password).IsValidPassword();

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}