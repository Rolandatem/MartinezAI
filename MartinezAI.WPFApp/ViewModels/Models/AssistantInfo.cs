namespace MartinezAI.WPFApp.ViewModels.Models;

public class AssistantInfo : BaseViewModel
{
	#region "Form Properties"
	public required string AssistantName
	{
		get => IsInDesignMode ? "Assistant Name" : field;
		set => OnPropertyChanged(ref field, value);
	} = "Assistant Name";
	#endregion
}
