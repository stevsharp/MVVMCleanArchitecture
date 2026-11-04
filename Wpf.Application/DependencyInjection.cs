using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using Wpf.Application.DTOs;
using Wpf.Application.Interfaces;
using Wpf.Application.Mapping;
using Wpf.Application.Services;
using Wpf.Application.Validators;

namespace Wpf.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<CustomerDto>, CustomerValidator>();
        services.AddSingleton<IValidator<OrderDto>, OrderValidator>();

        services.AddSingleton<CustomerMapper>();
        services.AddSingleton<OrderMapper>();

        services.AddTransient<ICustomerService, CustomerService>();

        return services;
    }
}
