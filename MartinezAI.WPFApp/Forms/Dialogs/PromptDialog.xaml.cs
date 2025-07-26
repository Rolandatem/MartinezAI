using MartinezAI.WPFApp.ViewModels.Dialogs;
using System.Windows;

namespace MartinezAI.WPFApp.Forms.Dialogs;

/// <summary>
/// Interaction logic for PromptDialog.xaml
/// </summary>
public partial class PromptDialog : Window
{
    public PromptDialog(PromptDialogViewModel vm)
    {
        InitializeComponent();
        this.DataContext = vm;

        vm.OnUserChoice += (result) =>
        {
            this.DialogResult = result;
            this.Close();
        };
    }
}
