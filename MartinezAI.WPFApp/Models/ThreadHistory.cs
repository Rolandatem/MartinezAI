using MartinezAI.WPFApp.Tools;
using MartinezAI.WPFApp.ViewModels;
using System.Collections.ObjectModel;

namespace MartinezAI.WPFApp.Models;

public class ThreadHistory : NotifyableClass
{
    public ObservableCollection<ChatLogMessage> Messages { get; set; } = new ObservableCollection<ChatLogMessage>();
    public int TokenCount
    {
        get => BaseViewModel.IsInDesignMode
            ? 10000
            : field;
        set => OnPropertyChanged(ref field, value);
    }
}
