using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Xml.Linq;

using Wpf.Application.DTOs;

namespace WpfAppCleanArchitecture.ViewModels;

public partial class CustomerDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private CustomerDto customer = new();

    [ObservableProperty]
    private bool dialogResult;

    [ObservableProperty]
    private string name = string.Empty;

    public bool IsNew => Customer.Id == 0;

    public bool CanSave => !string.IsNullOrWhiteSpace(name);

    partial void OnNameChanged(string value)
    {
        OnPropertyChanged(nameof(CanSave));
    }

    public CustomerDialogViewModel(CustomerDto customerDto)
    {
        Customer = customerDto;
        Name = customerDto.Name;
    }

    [RelayCommand]
    private void Save()
    {
        Customer.Name = Name;
        DialogResult = true;
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
    }
}
