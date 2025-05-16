using MartinezAI.WPFApp.Tools;
using MartinezAI.WPFApp.Windows.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MartinezAI.WPFApp.ViewModels;

public interface IMainWindowViewModel : IViewModel
{
    string AppTitle { get; }

    AsyncCommand StartChatCommand { get; }
}

public class MainWindowViewModelDesign : IMainWindowViewModel
{
    #region "IViewModel"
    public Window Window { get; set; }
    #endregion

    #region "Form Properties"
    public string AppTitle => "[APP TITLE]";
    #endregion

    #region "Commands"
    public AsyncCommand StartChatCommand { get; } = null;
    #endregion
}

public class MainWindowViewModel(
    IServiceProvider _serviceProvider) : BaseViewModel, IMainWindowViewModel
{
    #region "IViewModel"
    public Window Window { get; set; }
    #endregion

    #region "Form Properties"
    public string AppTitle => "Martinez AI";
    #endregion

    #region "Commands"
    public AsyncCommand StartChatCommand => new AsyncCommand(StartChatAsync);
    #endregion

    public Task StartChatAsync()
    {
        CreateChatDialog createChatDialog = _serviceProvider
            .GetRequiredService<CreateChatDialog>();

        createChatDialog.Owner = this.Window;
        createChatDialog.ShowDialog();
        return Task.CompletedTask;
    }
}
