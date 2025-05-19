using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MartinezAI.WPFApp.Tools;

public class NotifyableClass : INotifyPropertyChanged
{
    #region "Events"
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?
            .Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
