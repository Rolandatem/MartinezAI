using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MartinezAI.WPFApp.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
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
