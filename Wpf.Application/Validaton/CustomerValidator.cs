

using FluentValidation;

using Wpf.Application.DTOs;

namespace Wpf.Application.Validaton;

public class CustomerValidator : AbstractValidator<CustomerDto>
{
    public CustomerValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters.");
    }
}
