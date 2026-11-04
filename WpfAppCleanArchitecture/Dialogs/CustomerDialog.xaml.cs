using System.Windows;

using WpfAppCleanArchitecture.ViewModels;

namespace WpfAppCleanArchitecture.Dialogs;

public partial class CustomerDialog : Window
{
    public CustomerDialog(CustomerDialogViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
        viewModel.CloseRequested += result =>
        {
            if (DialogResult != result)
                DialogResult = result;
        };
    }
}
