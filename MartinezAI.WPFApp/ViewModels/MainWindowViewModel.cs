using MartinezAI.WPFApp.ExtensionMethods;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Tools;
using MartinezAI.WPFApp.Windows.Dialogs;
using MdXaml;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
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
    bool IsChatSelected { get; }

    AsyncCommand StartChatCommand { get; }
    AsyncCommand SendMessageCommand { get; }
    AsyncCommand<TextBox> NewLineCommand { get; }
    AsyncCommand<ChatHistoryContainer> CloseChatCommand { get; }

    Task InitializeChatAsync(
        string chatTitle,
        string chatContext);
}

public class MainWindowViewModelDesign : NotifyableClass, IMainWindowViewModel
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
        {
            ChatMessages = new ObservableCollection<ChatHistoryMessage>()
            {
                new ChatHistoryMessage("Assistant", "Hello! How can I help you?"),
                new ChatHistoryMessage("You", "Can you please tell me how to make a grilled cheese sandwich?")
            }
        }
    };
    public ChatHistoryContainer SelectedChat
    {
        get => this.ChatHistory[3];
        set { }
    }
    public bool IsChatSelected => this.SelectedChat != null;
    #endregion

    #region "Commands"
    public AsyncCommand StartChatCommand { get; } = null;
    public AsyncCommand SendMessageCommand { get; } = null;
    public AsyncCommand<TextBox> NewLineCommand { get; } = null;
    public AsyncCommand<ChatHistoryContainer> CloseChatCommand { get; } = null;
    #endregion

    #region "Public Methods"
    public Task InitializeChatAsync(string chatTitle, string chatContext) =>
        Task.CompletedTask;
    #endregion
}

public class MainWindowViewModel(
    IServiceProvider _serviceProvider,
    ChatClient _chatClient) : NotifyableClass, IMainWindowViewModel
{
    #region "Member Variables"
    readonly Markdown markdown = new Markdown();
    #endregion

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
            OnPropertyChanged(nameof(IsChatSelected));
        }
    }
    public bool IsChatSelected => this.SelectedChat != null;
    #endregion

    #region "Commands"
    public AsyncCommand StartChatCommand => new AsyncCommand(StartChatAsync);
    public AsyncCommand SendMessageCommand => new AsyncCommand(SendMessageAsync);
    public AsyncCommand<TextBox> NewLineCommand => new AsyncCommand<TextBox>(NewLineAsync);
    public AsyncCommand<ChatHistoryContainer> CloseChatCommand => new AsyncCommand<ChatHistoryContainer>(CloseChatAsync);
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

    private async Task SendMessageAsync()
    {
        this.SelectedChat.AddUserMessage(this.UserInput);

        ChatHistoryMessage newAssistantMessage = new ChatHistoryMessage("Assistant", String.Empty);
        this.SelectedChat.ChatMessages.Add(newAssistantMessage);

        await foreach (StreamingChatCompletionUpdate completionUpdate in
            _chatClient.CompleteChatStreamingAsync(
                this.SelectedChat.OpenAIChatHistory))
        {
            foreach (ChatMessageContentPart contentPart in completionUpdate.ContentUpdate)
            {
                newAssistantMessage.Message += contentPart.Text;
            }
        }

        this.SelectedChat.OpenAIChatHistory.Add(new AssistantChatMessage(newAssistantMessage.Message));
        this.UserInput = String.Empty;
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

    private Task CloseChatAsync(ChatHistoryContainer clickedChatHistory)
    {
        if (this.SelectedChat == null) { return Task.CompletedTask; }

        this.ChatHistory.Remove(clickedChatHistory);

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
        this.SelectedChat = newChat;

        return Task.CompletedTask;
    }
}
