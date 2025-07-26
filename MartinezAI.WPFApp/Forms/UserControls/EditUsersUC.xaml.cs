using MartinezAI.WPFApp.ViewModels.UserControls;
using System.Windows;
using System.Windows.Controls;

namespace MartinezAI.WPFApp.Forms.UserControls;

/// <summary>
/// Interaction logic for EditUsersUC.xaml
/// </summary>
public partial class EditUsersUC : UserControl
{
    public EditUsersUC()
    {
        InitializeComponent();

        Loaded += EditUsersUC_Loaded;
    }

    private async void EditUsersUC_Loaded(object sende, RoutedEventArgs e)
    {
        EditUsersUCViewModel vm = (EditUsersUCViewModel)this.DataContext;
        await vm.OnRefreshUsersCommand.ExecuteAsync();
    }
}
