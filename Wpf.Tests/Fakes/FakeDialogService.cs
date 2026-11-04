using Wpf.Application.DTOs;

using WpfAppCleanArchitecture.Services;

namespace Wpf.Tests.Fakes;

/// <summary>
/// Records every dialog interaction and returns scripted answers.
/// </summary>
public sealed class FakeDialogService : IDialogService
{
    /// <summary>What <see cref="ShowCustomerDialog"/> returns. Receives the DTO passed in (null for "new").</summary>
    public Func<CustomerDto?, CustomerDto?> CustomerDialogResult { get; set; } = _ => null;

    public bool ConfirmResult { get; set; } = true;

    public List<CustomerDto?> CustomerDialogCalls { get; } = [];
    public List<string> Confirmations { get; } = [];
    public List<string> Errors { get; } = [];
    public List<string> ValidationErrors { get; } = [];

    public CustomerDto? ShowCustomerDialog(CustomerDto? customer)
    {
        CustomerDialogCalls.Add(customer);
        return CustomerDialogResult(customer);
    }

    public bool Confirm(string message, string title)
    {
        Confirmations.Add(message);
        return ConfirmResult;
    }

    public void ShowError(string message, string title = "Error") => Errors.Add(message);

    public void ShowValidationErrors(IEnumerable<string> errors) => ValidationErrors.AddRange(errors);
}
