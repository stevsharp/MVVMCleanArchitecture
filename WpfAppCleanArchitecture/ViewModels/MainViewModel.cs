using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using Wpf.Application.DTOs;
using Wpf.Application.Exceptions;
using Wpf.Application.Interfaces;

using WpfAppCleanArchitecture.Services;

namespace WpfAppCleanArchitecture.ViewModels;

public partial class MainViewModel(ICustomerService customerService, IDialogService dialogService) : ObservableObject
{
    public ObservableCollection<CustomerDto> Customers { get; } = [];

    public ObservableCollection<OrderDto> Orders { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCustomerCommand), nameof(DeleteCustomerCommand))]
    private CustomerDto? selectedCustomer;

    private bool HasSelection => SelectedCustomer is not null;

    partial void OnSelectedCustomerChanged(CustomerDto? value)
        => _ = LoadOrdersAsync(value);

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task LoadCustomersAsync(CancellationToken cancellationToken)
    {
        try
        {
            var customers = await customerService.GetAllCustomersAsync(cancellationToken);

            Customers.Clear();
            foreach (var customer in customers)
                Customers.Add(customer);

            SelectedCustomer = null;
        }
        catch (OperationCanceledException)
        {
            // User cancelled; keep whatever was already loaded.
        }
        catch (Exception ex)
        {
            dialogService.ShowError($"Error loading customers: {ex.Message}");
        }
    }

    public async Task LoadOrdersAsync(CustomerDto? customer, CancellationToken cancellationToken = default)
    {
        if (customer is null)
        {
            Orders.Clear();
            return;
        }

        try
        {
            var orders = await customerService.GetOrdersByCustomerIdAsync(customer.Id, cancellationToken);

            // Selection may have moved on while we were awaiting; don't clobber the newer result.
            if (!ReferenceEquals(SelectedCustomer, customer)) return;

            Orders.Clear();
            foreach (var order in orders)
                Orders.Add(order);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            dialogService.ShowError($"Error loading orders: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddCustomerAsync(CancellationToken cancellationToken)
    {
        var draft = dialogService.ShowCustomerDialog(null);
        if (draft is null) return;

        try
        {
            var created = await customerService.AddCustomerAsync(draft, cancellationToken);

            Customers.Add(created);
            SelectedCustomer = created;
        }
        catch (ValidationException ex)
        {
            dialogService.ShowValidationErrors(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (Exception ex)
        {
            dialogService.ShowError($"Could not add customer: {ex.Message}");
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task EditCustomerAsync(CancellationToken cancellationToken)
    {
        var original = SelectedCustomer;
        if (original is null) return;

        var edited = dialogService.ShowCustomerDialog(original);
        if (edited is null || edited == original) return;

        try
        {
            await customerService.UpdateCustomerAsync(edited, cancellationToken);

            // DTOs are immutable, so swap the item to refresh the grid.
            var index = Customers.IndexOf(original);
            if (index >= 0)
                Customers[index] = edited;

            SelectedCustomer = edited;
        }
        catch (ValidationException ex)
        {
            dialogService.ShowValidationErrors(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (NotFoundException)
        {
            dialogService.ShowError("This customer no longer exists. Reload the list.");
        }
        catch (Exception ex)
        {
            dialogService.ShowError($"Could not update customer: {ex.Message}");
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task DeleteCustomerAsync(CancellationToken cancellationToken)
    {
        var customer = SelectedCustomer;
        if (customer is null) return;

        if (!dialogService.Confirm($"Delete {customer.Name}?", "Confirm"))
            return;

        try
        {
            await customerService.DeleteCustomerAsync(customer.Id, cancellationToken);

            Customers.Remove(customer);
            SelectedCustomer = null;
        }
        catch (NotFoundException)
        {
            // Already gone; just drop it from the list.
            Customers.Remove(customer);
            SelectedCustomer = null;
        }
        catch (Exception ex)
        {
            dialogService.ShowError($"Delete failed: {ex.Message}");
        }
    }
}
