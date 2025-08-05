using MartinezAI.WPFApp.Tools;

namespace MartinezAI.WPFApp.Models;

public class ChatLogMessage : NotifyableClass
{
    public required string Owner 
    {
        get => field;
        set => OnPropertyChanged(ref field, value);
    }
    public required string Content
    {
        get => field;
        set => OnPropertyChanged(ref field, value);
    }
    public bool? IsContentComplete
    {
        get => field;
        set => OnPropertyChanged(ref field, value);
    } = null;
}
