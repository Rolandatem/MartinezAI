using MartinezAI.WPFApp.Tools;

namespace MartinezAI.WPFApp.ViewModels.Dialogs;

public class NewThreadDialogViewModel : BaseViewModel
{
	#region "Events"
	public event Action<bool>? RequestClose;
	#endregion

	#region "Form Properties"
	public string NewThreadName
	{
		get => IsInDesignMode ? "[THREAD_NAME]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;
	#endregion

	#region "Commands"
	public AsyncCommand OnCreateCommand => new AsyncCommand(OnCreateCommandAsync);
	public AsyncCommand OnCancelCommand => new AsyncCommand(OnCancelCommandAsync);
	#endregion

	#region "Private Methods"
	private Task OnCreateCommandAsync()
	{
		this.RequestClose?.Invoke(true);
		return Task.CompletedTask;
	}
	private Task OnCancelCommandAsync()
	{
		this.RequestClose?.Invoke(false);
		return Task.CompletedTask;
	}
	#endregion
}
