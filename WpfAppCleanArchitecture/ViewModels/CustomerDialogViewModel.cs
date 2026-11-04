using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Wpf.Application.DTOs;

namespace WpfAppCleanArchitecture.ViewModels;

public partial class CustomerDialogViewModel : ObservableObject
{
    private readonly CustomerDto? _existing;

    public CustomerDialogViewModel(CustomerDto? existing)
    {
        _existing = existing;
        Name = existing?.Name ?? string.Empty;
    }

    /// <summary>Raised when the dialog should close. The argument is the dialog result.</summary>
    public event Action<bool>? CloseRequested;

    /// <summary>The customer to persist. Set only after <see cref="SaveCommand"/> runs.</summary>
    public CustomerDto? Result { get; private set; }

    public bool IsNew => _existing is null;

    public string Title => IsNew ? "New Customer" : "Edit Customer";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string name = string.Empty;

    public bool CanSave => !string.IsNullOrWhiteSpace(Name);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        var trimmed = Name.Trim();

        Result = _existing is null
            ? new CustomerDto(0, trimmed)
            : _existing with { Name = trimmed };

        CloseRequested?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(false);
}
