using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.ViewModels.Windows;
using System.Windows;

namespace MartinezAI.WPFApp.Forms.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(
        MainWindowViewModel vm,
        ISystemData systemData)
    {
        InitializeComponent();
        DataContext = vm;

        systemData.MainViewWindow = this;
        vm.RequestClose += () =>
        {
            this.Close();
        };
    }
}
