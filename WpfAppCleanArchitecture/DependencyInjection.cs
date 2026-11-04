using Microsoft.Extensions.DependencyInjection;

using WpfAppCleanArchitecture.Services;
using WpfAppCleanArchitecture.View;
using WpfAppCleanArchitecture.ViewModels;

namespace WpfAppCleanArchitecture;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSingleton<IDialogService, WpfDialogService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>(provider => new MainWindow
        {
            DataContext = provider.GetRequiredService<MainViewModel>()
        });

        return services;
    }
}
