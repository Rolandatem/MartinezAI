using MartinezAI.WPFApp.ViewModels;
using System.Windows;

namespace MartinezAI.WPFApp.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(IMainWindowViewModel viewModel)
    {
        InitializeComponent();
        viewModel.Window = this;
        DataContext = viewModel;
    }
}
