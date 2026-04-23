using FluentValidation;
using SmartApiary.Application.Common.Validators;


namespace SmartApiary.Application.Features.Users.Commands.ForgotPassword
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email).IsValidEmail();
        }
    }
}
