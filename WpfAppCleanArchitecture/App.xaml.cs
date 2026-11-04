using System.IO;
using System.Windows;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wpf.Application;
using Wpf.Infrastructure;

using WpfAppCleanArchitecture.View;
using WpfAppCleanArchitecture.ViewModels;

namespace WpfAppCleanArchitecture;

public partial class App : System.Windows.Application
{
    private const string AppFolderName = "WpfAppCleanArchitecture";

    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("Default");
                if (string.IsNullOrWhiteSpace(connectionString))
                    connectionString = DefaultConnectionString();

                services
                    .AddApplication()
                    .AddInfrastructure(connectionString)
                    .AddPresentation();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            await _host.StartAsync();
            await _host.Services.InitializeDatabaseAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            var viewModel = _host.Services.GetRequiredService<MainViewModel>();
            await viewModel.LoadCustomersCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"The application failed to start.\n\n{ex.Message}",
                "Startup error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(-1);
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            await _host.StopAsync();
        }
        finally
        {
            _host.Dispose();
        }

        base.OnExit(e);
    }

    /// <summary>Stores the database under %LocalAppData% so it does not depend on the working directory.</summary>
    private static string DefaultConnectionString()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppFolderName);

        Directory.CreateDirectory(folder);

        return $"Data Source={Path.Combine(folder, "app.db")}";
    }
}
