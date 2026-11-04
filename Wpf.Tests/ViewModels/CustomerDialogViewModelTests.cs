using Wpf.Application.DTOs;

using WpfAppCleanArchitecture.ViewModels;

namespace Wpf.Tests.ViewModels;

public class CustomerDialogViewModelTests
{
    [Fact]
    public void New_customer_starts_empty_and_cannot_save()
    {
        var vm = new CustomerDialogViewModel(null);

        Assert.True(vm.IsNew);
        Assert.Equal(string.Empty, vm.Name);
        Assert.False(vm.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void Existing_customer_prefills_name()
    {
        var vm = new CustomerDialogViewModel(new CustomerDto(7, "Alice"));

        Assert.False(vm.IsNew);
        Assert.Equal("Alice", vm.Name);
        Assert.True(vm.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void Typing_a_name_enables_save()
    {
        var vm = new CustomerDialogViewModel(null);
        var raised = false;
        vm.SaveCommand.CanExecuteChanged += (_, _) => raised = true;

        vm.Name = "Alice";

        Assert.True(raised);
        Assert.True(vm.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void Save_on_new_customer_produces_dto_with_zero_id_and_trimmed_name()
    {
        var vm = new CustomerDialogViewModel(null) { Name = "  Alice  " };
        bool? closeResult = null;
        vm.CloseRequested += r => closeResult = r;

        vm.SaveCommand.Execute(null);

        Assert.Equal(new CustomerDto(0, "Alice"), vm.Result);
        Assert.True(closeResult);
    }

    [Fact]
    public void Save_on_existing_customer_keeps_id()
    {
        var vm = new CustomerDialogViewModel(new CustomerDto(7, "Alice")) { Name = "Alicia" };

        vm.SaveCommand.Execute(null);

        Assert.Equal(new CustomerDto(7, "Alicia"), vm.Result);
    }

    [Fact]
    public void Cancel_requests_close_with_false_and_no_result()
    {
        var vm = new CustomerDialogViewModel(new CustomerDto(7, "Alice"));
        bool? closeResult = null;
        vm.CloseRequested += r => closeResult = r;

        vm.CancelCommand.Execute(null);

        Assert.False(closeResult);
        Assert.Null(vm.Result);
    }
}
