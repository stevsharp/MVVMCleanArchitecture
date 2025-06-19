using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

using Wpf.Application.DTOs;
using Wpf.Domain.Entities;


using WpfAppCleanArchitecture.Dialogs;

public partial class MainViewModel(ICustomerService customerService) : ObservableObject
{
    private readonly ICustomerService _customerService = customerService;

    [ObservableProperty]
    private ObservableCollection<CustomerDto> customers = new();

    [ObservableProperty]
    private CustomerDto? selectedCustomer;

    [ObservableProperty]
    private ObservableCollection<string> validationMessages = new();

    [RelayCommand]
    public async Task LoadCustomersAsync()
    {
        try
        {
            Customers.Clear();
            var result = await _customerService.GetAllCustomersAsync(CancellationToken.None);
            foreach (var customer in result)
                Customers.Add(customer);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading customers: " + ex.Message);
        }
    }

    [RelayCommand]
    public async Task AddCustomerAsync()
    {
        var newCustomer = new CustomerDto();
        var dialog = new CustomerDialog(newCustomer);
        
        if (dialog.ShowDialog() == true)
        {
            try
            {
                newCustomer.Name = dialog.Customer.Name; // Assuming dialog returns name 
                var id = await _customerService.AddCustomerAsync(newCustomer, CancellationToken.None);
                newCustomer.Id = id;

                Customers.Add(newCustomer);
            }
            catch (ValidationException ex)
            {
                ShowValidationErrors(ex);
            }
        }
    }

    [RelayCommand]
    public async Task EditCustomerAsync()
    {
        if (SelectedCustomer is null) return;

        var copy = new CustomerDto
        {
            Id = SelectedCustomer.Id,
            Name = SelectedCustomer.Name
        };

        var dialog = new CustomerDialog(copy);
        if (dialog.ShowDialog() == true)
        {
            try
            {
                await _customerService.UpdateCustomerAsync(copy, CancellationToken.None);
                SelectedCustomer.Name = copy.Name;
            }
            catch (ValidationException ex)
            {
                ShowValidationErrors(ex);
            }
        }
    }

    [RelayCommand]
    public async Task DeleteCustomerAsync()
    {
        if (SelectedCustomer is null) return;

        var result = MessageBox.Show($"Delete {SelectedCustomer.Name}?", "Confirm", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _customerService.DeleteCustomerAsync(SelectedCustomer.Id, CancellationToken.None);
            Customers.Remove(SelectedCustomer);
            SelectedCustomer = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Delete failed: " + ex.Message);
        }
    }

    private void ShowValidationErrors(ValidationException ex)
    {
        var message = string.Join("\n", ex.Errors
               .Select(error => $"• {error.PropertyName}: {error.ErrorMessage}"));

        MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}