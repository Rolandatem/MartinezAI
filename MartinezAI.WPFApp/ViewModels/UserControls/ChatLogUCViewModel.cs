using MartinezAI.WPFApp.Models;
using System.Collections.ObjectModel;

namespace MartinezAI.WPFApp.ViewModels.UserControls;

public class ChatLogUCViewModel : BaseViewModel
{
	#region "Form Properties"
	public ObservableCollection<ChatLogMessage> ChatLogMessages
	{
		get
		{
			if (IsInDesignMode)
			{
				return new ObservableCollection<ChatLogMessage>(
					[
						new ChatLogMessage() { Owner = "User", Content = "<span>Some Message</span>" },
						new ChatLogMessage() { Owner = "AI Assistant", Content = "<span>AI Assistant Response." }
					]);
			}

			return field;
		}
		set => OnPropertyChanged(ref field, value);
	} = new ObservableCollection<ChatLogMessage>();
	#endregion
}
