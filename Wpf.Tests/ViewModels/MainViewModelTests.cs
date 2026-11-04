using FluentValidation;
using FluentValidation.Results;

using Wpf.Application.DTOs;
using Wpf.Tests.Fakes;

using WpfAppCleanArchitecture.ViewModels;

namespace Wpf.Tests.ViewModels;

public class MainViewModelTests
{
    private readonly FakeCustomerService _service = new();
    private readonly FakeDialogService _dialogs = new();
    private readonly MainViewModel _vm;

    public MainViewModelTests()
    {
        _vm = new MainViewModel(_service, _dialogs);
    }

    [Fact]
    public async Task LoadCustomers_fills_collection_and_clears_selection()
    {
        _service.Seed("Alice");
        _service.Seed("Bob");

        await _vm.LoadCustomersCommand.ExecuteAsync(null);

        Assert.Equal(["Alice", "Bob"], _vm.Customers.Select(c => c.Name));
        Assert.Null(_vm.SelectedCustomer);
        Assert.Empty(_dialogs.Errors);
    }

    [Fact]
    public async Task LoadCustomers_reports_failure_through_dialog_service()
    {
        _service.ThrowOnNextCall = new InvalidOperationException("db down");

        await _vm.LoadCustomersCommand.ExecuteAsync(null);

        var error = Assert.Single(_dialogs.Errors);
        Assert.Contains("db down", error);
    }

    [Fact]
    public async Task Selecting_a_customer_loads_its_orders()
    {
        var alice = _service.Seed("Alice", "A1", "A2");

        _vm.SelectedCustomer = alice;
        await _vm.LoadOrdersAsync(alice);

        Assert.Equal(["A1", "A2"], _vm.Orders.Select(o => o.Description));
    }

    [Fact]
    public async Task Orders_are_discarded_if_selection_changed_while_loading()
    {
        var alice = _service.Seed("Alice", "A1");
        var bob = _service.Seed("Bob", "B1");

        _vm.SelectedCustomer = bob;
        await _vm.LoadOrdersAsync(alice);

        Assert.Equal(["B1"], _vm.Orders.Select(o => o.Description));
    }

    [Fact]
    public async Task Clearing_selection_clears_orders()
    {
        var alice = _service.Seed("Alice", "A1");
        _vm.SelectedCustomer = alice;
        await _vm.LoadOrdersAsync(alice);

        await _vm.LoadOrdersAsync(null);

        Assert.Empty(_vm.Orders);
    }

    [Fact]
    public void Edit_and_Delete_require_a_selection()
    {
        Assert.False(_vm.EditCustomerCommand.CanExecute(null));
        Assert.False(_vm.DeleteCustomerCommand.CanExecute(null));

        _vm.SelectedCustomer = new CustomerDto(1, "Alice");

        Assert.True(_vm.EditCustomerCommand.CanExecute(null));
        Assert.True(_vm.DeleteCustomerCommand.CanExecute(null));
    }

    [Fact]
    public async Task AddCustomer_adds_created_dto_and_selects_it()
    {
        _dialogs.CustomerDialogResult = _ => new CustomerDto(0, "Alice");

        await _vm.AddCustomerCommand.ExecuteAsync(null);

        var added = Assert.Single(_vm.Customers);
        Assert.Equal("Alice", added.Name);
        Assert.NotEqual(0, added.Id);
        Assert.Same(added, _vm.SelectedCustomer);
        Assert.Equal([null], _dialogs.CustomerDialogCalls);
    }

    [Fact]
    public async Task AddCustomer_does_nothing_when_dialog_is_cancelled()
    {
        _dialogs.CustomerDialogResult = _ => null;

        await _vm.AddCustomerCommand.ExecuteAsync(null);

        Assert.Empty(_vm.Customers);
    }

    [Fact]
    public async Task AddCustomer_shows_validation_errors()
    {
        _dialogs.CustomerDialogResult = _ => new CustomerDto(0, "");
        _service.ThrowOnNextCall = new ValidationException([new ValidationFailure("Name", "Customer name is required.")]);

        await _vm.AddCustomerCommand.ExecuteAsync(null);

        Assert.Equal(["Customer name is required."], _dialogs.ValidationErrors);
        Assert.Empty(_vm.Customers);
    }

    [Fact]
    public async Task EditCustomer_replaces_item_in_collection()
    {
        var alice = _service.Seed("Alice");
        await _vm.LoadCustomersCommand.ExecuteAsync(null);
        _vm.SelectedCustomer = _vm.Customers[0];
        _dialogs.CustomerDialogResult = original => original! with { Name = "Alicia" };

        await _vm.EditCustomerCommand.ExecuteAsync(null);

        var item = Assert.Single(_vm.Customers);
        Assert.Equal(new CustomerDto(alice.Id, "Alicia"), item);
        Assert.Same(item, _vm.SelectedCustomer);
        Assert.Equal("Alicia", _service.Customers.Single().Name);
    }

    [Fact]
    public async Task EditCustomer_skips_service_when_nothing_changed()
    {
        _service.Seed("Alice");
        await _vm.LoadCustomersCommand.ExecuteAsync(null);
        _vm.SelectedCustomer = _vm.Customers[0];
        _dialogs.CustomerDialogResult = original => original;
        _service.ThrowOnNextCall = new InvalidOperationException("should not be called");

        await _vm.EditCustomerCommand.ExecuteAsync(null);

        Assert.Empty(_dialogs.Errors);
        Assert.NotNull(_service.ThrowOnNextCall);
    }

    [Fact]
    public async Task DeleteCustomer_removes_item_after_confirmation()
    {
        _service.Seed("Alice");
        await _vm.LoadCustomersCommand.ExecuteAsync(null);
        _vm.SelectedCustomer = _vm.Customers[0];
        _dialogs.ConfirmResult = true;

        await _vm.DeleteCustomerCommand.ExecuteAsync(null);

        Assert.Empty(_vm.Customers);
        Assert.Null(_vm.SelectedCustomer);
        Assert.Empty(_service.Customers);
        Assert.Single(_dialogs.Confirmations);
    }

    [Fact]
    public async Task DeleteCustomer_keeps_item_when_not_confirmed()
    {
        _service.Seed("Alice");
        await _vm.LoadCustomersCommand.ExecuteAsync(null);
        _vm.SelectedCustomer = _vm.Customers[0];
        _dialogs.ConfirmResult = false;

        await _vm.DeleteCustomerCommand.ExecuteAsync(null);

        Assert.Single(_vm.Customers);
        Assert.Single(_service.Customers);
    }
}
