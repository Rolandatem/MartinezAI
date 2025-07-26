using MartinezAI.WPFApp.ViewModels.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MartinezAI.WPFApp.Forms.UserControls;

/// <summary>
/// Interaction logic for LoginUC.xaml
/// </summary>
public partial class LoginUC : UserControl
{
    public LoginUC()
    {
        InitializeComponent();

        Loaded += LoginUC_Loaded;
    }

    public async void SignIn_Click(object sender, RoutedEventArgs e)
    {
        await (DataContext as LoginUCViewModel)!
            .SignInCommand
            .ExecuteAsync(passwordBox.Password);
    }

    public async void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await (DataContext as LoginUCViewModel)!
                .SignInCommand
                .ExecuteAsync(passwordBox.Password);
        }
    }

    public void LoginUC_Loaded(object sender, RoutedEventArgs e)
    {
        if ((DataContext as LoginUCViewModel)?.Email.Exists() == true)
        {
            passwordBox.Focus();
        }
        else
        {
            emailTextBox.Focus();
        }
    }
}
