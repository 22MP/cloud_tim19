using FluentValidation;
using SmartApiary.Application.Common.Validators;

namespace SmartApiary.Application.Features.Users.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).IsValidEmail();

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}