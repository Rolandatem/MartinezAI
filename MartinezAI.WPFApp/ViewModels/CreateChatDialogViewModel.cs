namespace MartinezAI.WPFApp.ViewModels;

public interface ICreateChatDialogViewModel
{
    string ChatTitle { get; set; }
    string ChatContext { get; set; }
}

public class CreateChatDialogViewModelDesign : ICreateChatDialogViewModel
{
    public string ChatTitle { get; set; } = "[CHAT TITLE]";
    public string ChatContext { get; set; } = "[CHAT CONTEXT]";
}

public class CreateChatDialogViewModel : BaseViewModel, ICreateChatDialogViewModel
{
    public string ChatTitle
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string ChatContext { get; set; }
}
