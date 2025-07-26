using MartinezAI.WPFApp.ViewModels.Dialogs;
using System.Windows;

namespace MartinezAI.WPFApp.Forms.Dialogs;

/// <summary>
/// Interaction logic for NewThreadDialog.xaml
/// </summary>
public partial class NewThreadDialog : Window
{
    public NewThreadDialog(NewThreadDialogViewModel vm)
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
