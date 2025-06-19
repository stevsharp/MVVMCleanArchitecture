

using FluentValidation;

using Wpf.Application.DTOs;

namespace Wpf.Application.Validaton;

public class OrderValidator : AbstractValidator<OrderDto>
{
    public OrderValidator()
    {
        RuleFor(o => o.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");

        RuleFor(o => o.CustomerId)
            .GreaterThan(0)
            .WithMessage("Order must be linked to a valid customer.");
    }
}
