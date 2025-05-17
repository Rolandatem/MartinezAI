using MartinezAI.WPFApp.ExtensionMethods;

namespace MartinezAI.WPFApp.Tools;

public class ChatHistoryContainer
{
    #region "Public Properties"
    public string ChatTitle { get; set; }
    public string ChatContext { get; set; }

    public List<string> ChatHistory { get; set; } = new List<string>();
    #endregion

    #region "Constructors"
    public ChatHistoryContainer() { }
    public ChatHistoryContainer(
        string chatTitle,
        string chatContext = null)
    {
        this.ChatTitle = chatTitle.ValueIfEmptyOrNull("no chat history");
        this.ChatContext = chatContext;
    }
    #endregion
}
