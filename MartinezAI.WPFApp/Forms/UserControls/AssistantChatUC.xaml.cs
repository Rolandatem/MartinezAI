using MartinezAI.WPFApp.ViewModels.UserControls;
using System.Windows.Controls;

namespace MartinezAI.WPFApp.Forms.UserControls;

/// <summary>
/// Interaction logic for AssistantChatUC.xaml
/// </summary>
public partial class AssistantChatUC : UserControl
{
    public AssistantChatUC()
    {
        InitializeComponent();
        
        Loaded += AssistantChatUC_Loaded;
    }

    private async void AssistantChatUC_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await (this.DataContext as AssistantChatUCViewModel)!
            .OnChatLoadedAsync();
    }
}
