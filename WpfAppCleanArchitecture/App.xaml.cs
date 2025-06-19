

using FluentValidation;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Windows;

using Wpf.Application.DTOs;
using Wpf.Application.Validaton;
using Wpf.Infrastructure.Data;
using Wpf.Infrastructure.Mapping;

using WpfAppCleanArchitecture.View;

namespace WpfAppCleanArchitecture
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; } = null!;

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    string connectionString = context.Configuration.GetConnectionString("Default")
                                              ?? "Data Source=app.db";

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=app.db"));

                    services.AddDbContextFactory<AppDbContext>(options =>
                        options.UseSqlite("Data Source=app.db"));

                    services.AddSingleton<IValidator<CustomerDto>, CustomerValidator>();
                    services.AddSingleton<IValidator<OrderDto>, OrderValidator>();

                    services.AddScoped<ICustomerService, CustomerService>();

                    services.AddSingleton<CustomerMapper>();
                    services.AddSingleton<OrderMapper>();

                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>(provider =>
                    {
                        var vm = provider.GetRequiredService<MainViewModel>();
                        return new MainWindow { DataContext = vm };
                    });
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            using (var scope = AppHost.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.Migrate(); // ✅ This ensures schema is created/applied

                AppDbSeeder.Seed(db); // ✅ Seed after migration
            }


            await AppHost.StartAsync();
            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            AppHost.Dispose();
            base.OnExit(e);
        }
    }

}
