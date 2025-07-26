using MartinezAI.WPFApp.ViewModels.Dialogs;
using System.Windows;

namespace MartinezAI.WPFApp.Forms.Dialogs;

/// <summary>
/// Interaction logic for CreateAssistantDialog.xaml
/// </summary>
public partial class UpsertAssistantDialog : Window
{
    public UpsertAssistantDialog(UpsertAssistantDialogViewModel vm)
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
