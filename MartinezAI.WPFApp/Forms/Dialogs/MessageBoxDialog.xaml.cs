using MartinezAI.WPFApp.ViewModels.Dialogs;
using System.Windows;

namespace MartinezAI.WPFApp.Forms.Dialogs;

/// <summary>
/// Interaction logic for MessageBoxDialog.xaml
/// </summary>
public partial class MessageBoxDialog : Window
{
    public MessageBoxDialog(MessageBoxDialogViewModel vm)
    {
        InitializeComponent();
        this.DataContext = vm;

        vm.RequestClose += () =>
        {
            this.Close();
        };

        Loaded += Dialog_Loaded;
    }

    public void Dialog_Loaded(object sender, RoutedEventArgs e)
    {
        OkButton.Focus();
    }
}
