using MartinezAI.WPFApp.ViewModels.Dialogs;
using System.Windows;

namespace MartinezAI.WPFApp.Forms.Dialogs;

/// <summary>
/// Interaction logic for CreateUserDialog.xaml
/// </summary>
public partial class CreateUserDialog : Window
{
    public CreateUserDialog(CreateUserDialogViewModel vm)
    {
        InitializeComponent();
        this.DataContext = vm;

        vm.RequestClose += (result) =>
        {
            this.DialogResult = result;
            this.Close();
        };
    }
}
