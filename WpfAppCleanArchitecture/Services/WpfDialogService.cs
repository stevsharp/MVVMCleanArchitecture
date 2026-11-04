using System.Windows;

using Wpf.Application.DTOs;

using WpfAppCleanArchitecture.Dialogs;
using WpfAppCleanArchitecture.ViewModels;

namespace WpfAppCleanArchitecture.Services;

public sealed class WpfDialogService : IDialogService
{
    public CustomerDto? ShowCustomerDialog(CustomerDto? customer)
    {
        var viewModel = new CustomerDialogViewModel(customer);
        var dialog = new CustomerDialog(viewModel) { Owner = ActiveWindow };

        return dialog.ShowDialog() == true ? viewModel.Result : null;
    }

    public bool Confirm(string message, string title)
        => MessageBox.Show(ActiveWindow, message, title, MessageBoxButton.YesNo, MessageBoxImage.Question)
           == MessageBoxResult.Yes;

    public void ShowError(string message, string title = "Error")
        => MessageBox.Show(ActiveWindow, message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    public void ShowValidationErrors(IEnumerable<string> errors)
    {
        var message = string.Join(Environment.NewLine, errors.Select(e => $"• {e}"));
        MessageBox.Show(ActiveWindow, message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private static Window ActiveWindow
        => System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
           ?? System.Windows.Application.Current.MainWindow;
}
