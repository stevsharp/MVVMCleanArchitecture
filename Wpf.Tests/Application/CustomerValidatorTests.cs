using Wpf.Application.DTOs;
using Wpf.Application.Validators;

namespace Wpf.Tests.Application;

public class CustomerValidatorTests
{
    private readonly CustomerValidator _validator = new();

    [Fact]
    public void Valid_customer_passes()
    {
        var result = _validator.Validate(new CustomerDto(1, "Alice"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Blank_name_fails(string name)
    {
        var result = _validator.Validate(new CustomerDto(1, name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CustomerDto.Name));
    }

    [Fact]
    public void Name_longer_than_max_fails()
    {
        var name = new string('x', CustomerValidator.NameMaxLength + 1);

        var result = _validator.Validate(new CustomerDto(1, name));

        Assert.False(result.IsValid);
    }
}
