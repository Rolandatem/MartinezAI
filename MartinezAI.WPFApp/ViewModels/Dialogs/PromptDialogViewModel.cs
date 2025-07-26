using MartinezAI.WPFApp.Tools;

namespace MartinezAI.WPFApp.ViewModels.Dialogs;

public class PromptDialogViewModel : BaseViewModel
{
	#region "Events"
	public event Action<bool>? OnUserChoice;
	#endregion

	#region "Form Properties"
	public required string PromptMessage
	{
		get => IsInDesignMode ? "[PROMPT_MESSAGE]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;

	public required string YesButtonText
	{
		get => IsInDesignMode ? "[YES]" : field;
		set => OnPropertyChanged(ref field, value);
	} = "Yes";

	public required string NoButtonText
	{
		get => IsInDesignMode ? "[NO]" : field;
		set => OnPropertyChanged(ref field, value);
	} = "No";
	#endregion

	#region "Commands"
	public AsyncCommand OnYesCommand => new AsyncCommand(OnYesCommandAsync);
	public AsyncCommand OnNoCommand => new AsyncCommand(OnNoCommandAsync);
	#endregion

	#region "Private Methods"
	public Task OnYesCommandAsync()
	{
		OnUserChoice?.Invoke(true);
		return Task.CompletedTask;
	}

	public Task OnNoCommandAsync()
	{
		OnUserChoice?.Invoke(false);
		return Task.CompletedTask;
	}
	#endregion
}
