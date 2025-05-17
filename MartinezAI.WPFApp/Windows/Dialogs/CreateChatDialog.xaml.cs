using MartinezAI.WPFApp.ViewModels;
using System.Windows;

namespace MartinezAI.WPFApp.Windows.Dialogs;

public partial class CreateChatDialog : Window
{
    public CreateChatDialog(ICreateChatDialogViewModel viewModel)
    {
        InitializeComponent();
        viewModel.Window = this;
        DataContext = viewModel;
    }
}
