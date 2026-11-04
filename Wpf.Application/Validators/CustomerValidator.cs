using FluentValidation;

using Wpf.Application.DTOs;

namespace Wpf.Application.Validators;

public sealed class CustomerValidator : AbstractValidator<CustomerDto>
{
    public const int NameMaxLength = 200;

    public CustomerValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(NameMaxLength)
            .WithMessage($"Name cannot exceed {NameMaxLength} characters.");
    }
}
