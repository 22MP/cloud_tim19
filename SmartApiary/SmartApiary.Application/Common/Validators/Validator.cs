using FluentValidation;
namespace SmartApiary.Application.Common.Validators;

internal static class Validator
{
    // For email validation
    public static IRuleBuilderOptions<T, string> IsValidEmail<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }

    // For password validation
    public static IRuleBuilderOptions<T, string> IsValidPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");
    }
}