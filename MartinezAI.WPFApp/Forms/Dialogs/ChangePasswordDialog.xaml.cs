using MartinezAI.WPFApp.ViewModels.Dialogs;
using System.Windows;
using System.Windows.Input;

namespace MartinezAI.WPFApp.Forms.Dialogs;

/// <summary>
/// Interaction logic for ChangePasswordDialog.xaml
/// </summary>
public partial class ChangePasswordDialog : Window
{
    public ChangePasswordDialog(ChangePasswordDialogViewModel vm)
    {
        InitializeComponent();
        this.DataContext = vm;

        vm.RequestClose += (result) =>
        {
            this.DialogResult = result;
            this.Close();
        };

        Loaded += Dialog_Loaded;
    }

    public void Dialog_Loaded(object sender, RoutedEventArgs e)
    {
        NewPasswordBox.Focus();
    }

    public async void Update_Click(object sender, RoutedEventArgs e)
    {
        await (DataContext as ChangePasswordDialogViewModel)!
            .OnUpdateCommandAsync(
                NewPasswordBox.Password,
                ConfirmPasswordBox.Password);
    }

    public void ConfirmPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Update_Click(sender, e);
        }
    }
}
