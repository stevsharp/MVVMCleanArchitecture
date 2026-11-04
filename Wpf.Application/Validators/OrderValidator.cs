using FluentValidation;

using Wpf.Application.DTOs;

namespace Wpf.Application.Validators;

public sealed class OrderValidator : AbstractValidator<OrderDto>
{
    public const int DescriptionMaxLength = 500;

    public OrderValidator()
    {
        RuleFor(o => o.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(DescriptionMaxLength)
            .WithMessage($"Description cannot exceed {DescriptionMaxLength} characters.");

        RuleFor(o => o.CustomerId)
            .GreaterThan(0)
            .WithMessage("Order must be linked to a valid customer.");
    }
}
