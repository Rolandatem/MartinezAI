using MartinezAI.WPFApp.ViewModels.UserControls;
using System.Windows.Controls;

namespace MartinezAI.WPFApp.Forms.UserControls;

/// <summary>
/// Interaction logic for WorkspaceUC.xaml
/// </summary>
public partial class WorkspaceUC : UserControl
{
    public WorkspaceUC()
    {
        InitializeComponent();

        Loaded += WorkspaceUC_Loaded;
    }

    private async void WorkspaceUC_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        WorkspaceUCViewModel vm = (WorkspaceUCViewModel)this.DataContext!;
        await vm.OnLoadAssistantsCommand.ExecuteAsync();
    }
}
