using MartinezAI.WPFApp.ExtensionMethods;
using MartinezAI.WPFApp.Tools;
using System.Windows;

namespace MartinezAI.WPFApp.ViewModels;

public interface ICreateChatDialogViewModel : IViewModel
{
    string ChatTitle { get; set; }
    string ChatContext { get; set; }

    AsyncCommand StartChatCommand { get; }
    AsyncCommand CancelCreateChatCommand { get; }
}

public class CreateChatDialogViewModelDesign : ICreateChatDialogViewModel
{
    public Window Window { get; set; }

    public string ChatTitle { get; set; } = "[CHAT TITLE]";
    public string ChatContext { get; set; } = "[CHAT CONTEXT]";

    public AsyncCommand StartChatCommand { get; } = null;
    public AsyncCommand CancelCreateChatCommand { get; } = null;
}

public class CreateChatDialogViewModel(
    IMainWindowViewModel _mainWindowViewModel) 
    : BaseViewModel, ICreateChatDialogViewModel
{
    #region "IViewModel"
    public Window Window { get; set; }
    #endregion

    #region "Form Properties"
    public string ChatTitle
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public string ChatContext
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region "AsyncCommands"
    public AsyncCommand StartChatCommand => new AsyncCommand(StartChatAsync);
    public AsyncCommand CancelCreateChatCommand => new AsyncCommand(CancelCreateChatAsync);
    #endregion

    private async Task StartChatAsync()
    {
        await _mainWindowViewModel.InitializeChatAsync(
            this.ChatTitle.NullIfEmpty(),
            this.ChatContext.NullIfEmpty());

        await this.CancelCreateChatAsync();
    }

    private Task CancelCreateChatAsync()
    {
        this.Window.Close();
        return Task.CompletedTask;
    }
}
