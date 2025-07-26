using MartinezAI.WPFApp.Enums;
using MartinezAI.WPFApp.Tools;

namespace MartinezAI.WPFApp.ViewModels.Dialogs;

public class MessageBoxDialogViewModel : BaseViewModel
{
	#region "Events"
	public event Action? RequestClose;
	#endregion

	#region "Form Properties"
	public required string Title
	{
		get => IsInDesignMode ? "[TITLE]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;

	public required string Message
	{
		get => IsInDesignMode ? "[Message]" : field;
		set => OnPropertyChanged(ref field, value);
	}

	public MessageBoxIconFlag IconFlag
	{
		get => IsInDesignMode ? MessageBoxIconFlag.Info : field;
		set
		{
			OnPropertyChanged(ref field, value);

			this.Title = value switch
			{
				MessageBoxIconFlag.Warning => "Warning",
				MessageBoxIconFlag.Error => "Error",
				_ => "Information"
			};
			OnPropertyChanged(() => Title);
		}
	}
	#endregion

	#region "Commands"
	public AsyncCommand OnCloseCommand => new AsyncCommand(OnCloseCommandAsync);
	#endregion

	#region "Private Methods"
	public Task OnCloseCommandAsync()
	{
		RequestClose?.Invoke();
		return Task.CompletedTask;
	}
	#endregion
}
