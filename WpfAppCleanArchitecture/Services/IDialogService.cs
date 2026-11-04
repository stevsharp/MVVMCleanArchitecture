using Wpf.Application.DTOs;

namespace WpfAppCleanArchitecture.Services;

/// <summary>
/// Abstracts every modal interaction so view models can be unit-tested without a UI.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows the customer editor. Pass <c>null</c> to create a new customer.
    /// Returns the edited customer, or <c>null</c> if the user cancelled.
    /// </summary>
    CustomerDto? ShowCustomerDialog(CustomerDto? customer);

    bool Confirm(string message, string title);

    void ShowError(string message, string title = "Error");

    void ShowValidationErrors(IEnumerable<string> errors);
}
