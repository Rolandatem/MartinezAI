using MartinezAI.WPFApp.ExtensionMethods;
using MartinezAI.WPFApp.Tools;
using MartinezAI.WPFApp.Windows.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MartinezAI.WPFApp.ViewModels;

public interface IMainWindowViewModel : IViewModel
{
    string AppTitle { get; }
    string UserInput { get; set; }
    ObservableCollection<ChatHistoryContainer> ChatHistory { get; }
    ChatHistoryContainer SelectedChat { get; set; }

    AsyncCommand StartChatCommand { get; }
    AsyncCommand SendMessageCommand { get; }
    AsyncCommand<TextBox> NewLineCommand { get; }

    Task InitializeChatAsync(
        string chatTitle,
        string chatContext);
}

public class MainWindowViewModelDesign : BaseViewModel, IMainWindowViewModel
{
    #region "IViewModel"
    public Window Window { get; set; }
    #endregion

    #region "Form Properties"
    public string AppTitle => "[APP TITLE]";
    public string UserInput { get; set; } = "[USER INPUT]";
    public ObservableCollection<ChatHistoryContainer> ChatHistory => new ObservableCollection<ChatHistoryContainer>()
    {
        new ChatHistoryContainer("Test Chat 1"),
        new ChatHistoryContainer("Test Chat 2"),
        new ChatHistoryContainer("Test Chat 3"),
        new ChatHistoryContainer("Test Chat 4 with a really long chat title so that we can test trimming","""
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 0123456789 
            """)
    };
    public ChatHistoryContainer SelectedChat
    {
        get => this.ChatHistory[3];
        set { }
    }
    #endregion

    #region "Commands"
    public AsyncCommand StartChatCommand { get; } = null;
    public AsyncCommand SendMessageCommand { get; } = null;
    public AsyncCommand<TextBox> NewLineCommand { get; } = null;
    #endregion

    #region "Public Methods"
    public Task InitializeChatAsync(string chatTitle, string chatContext) =>
        Task.CompletedTask;
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
    public string UserInput
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<ChatHistoryContainer> ChatHistory
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = new ObservableCollection<ChatHistoryContainer>();
    public ChatHistoryContainer SelectedChat
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region "Commands"
    public AsyncCommand StartChatCommand => new AsyncCommand(StartChatAsync);
    public AsyncCommand SendMessageCommand => new AsyncCommand(SendMessageAsync);
    public AsyncCommand<TextBox> NewLineCommand => new AsyncCommand<TextBox>(NewLineAsync);
    #endregion

    #region "Private Methods"
    private Task StartChatAsync()
    {
        CreateChatDialog createChatDialog = _serviceProvider
            .GetRequiredService<CreateChatDialog>();

        createChatDialog.Owner = this.Window;
        createChatDialog.ShowDialog();
        return Task.CompletedTask;
    }

    private Task SendMessageAsync()
    {
        this.ChatHistory.Add(new ChatHistoryContainer(this.UserInput));
        return Task.CompletedTask;
    }
    private Task NewLineAsync(TextBox textBox)
    {
        if (textBox == null) { return Task.CompletedTask; }

        int caretIdx = textBox.CaretIndex;

        if (this.UserInput.IsEmpty())
        {
            this.UserInput += "\r";
        }
        else
        {
            this.UserInput = this.UserInput.Insert(caretIdx, "\r");
        }

        textBox.CaretIndex = caretIdx + 1;

        return Task.CompletedTask;
    }
    #endregion

    public Task InitializeChatAsync(
        string chatTitle,
        string chatContext)
    {
        ChatHistoryContainer newChat = new ChatHistoryContainer(chatTitle, chatContext);
        this.ChatHistory.Add(newChat);

        //--Make new chat the active chat.

        return Task.CompletedTask;
    }
}
