using MartinezAI.WPFApp.ExtensionMethods;
using OpenAI.Chat;
using System.Collections.ObjectModel;

namespace MartinezAI.WPFApp.Tools;

public class ChatHistoryContainer
{
    #region "Public Properties"
    public string ChatTitle { get; set; }
    public string ChatContext { get; set; }

    public ObservableCollection<ChatHistoryMessage> ChatMessages { get; set; } = new ObservableCollection<ChatHistoryMessage>();
    public List<ChatMessage> OpenAIChatHistory { get; set; } = new List<ChatMessage>();
    #endregion

    #region "Constructors"
    public ChatHistoryContainer() { }
    public ChatHistoryContainer(
        string chatTitle,
        string chatContext = null)
    {
        this.ChatTitle = chatTitle.ValueIfEmptyOrNull("no chat history");
        this.ChatContext = chatContext;

        if (this.ChatContext.HasValue())
        {
            this.OpenAIChatHistory.Add(new SystemChatMessage(this.ChatContext));
        }
    }
    #endregion

    #region "Public Methods"
    public void AddUserMessage(string userMessage)
    {
        this.ChatMessages.Add(new ChatHistoryMessage(
            "You",
            userMessage));

        this.OpenAIChatHistory.Add(new UserChatMessage(userMessage));
    }
    #endregion
}

public class ChatHistoryMessage : NotifyableClass
{
    public string Entity
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string Message
    {
        get;
        set
        {
            field = value; 
            OnPropertyChanged();
        }
    }

    public ChatHistoryMessage() { }
    public ChatHistoryMessage(
        string entity,
        string message)
    {
        this.Entity = entity;
        this.Message = message;
    }
}
