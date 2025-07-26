using MartinezAI.WPFApp.Tools;

namespace MartinezAI.WPFApp.ViewModels.Dialogs;

public class UpsertAssistantDialogViewModel : BaseViewModel
{
	#region "Events"
	public event Action<bool>? RequestClose;
	#endregion

	#region "Form Properties"
	public required string DialogTitle
	{
		get => IsInDesignMode ? "[DIALOG TITLE]" : field;
		set => OnPropertyChanged(ref field, value);
	} = "Create New Assistant";

	public required string AssistantName
	{
		get => IsInDesignMode ? "[ASSISTANT NAME]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;

	public required string SystemMessage
	{
		get => IsInDesignMode ? "[SYSTEM MESSAGE]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;

	public bool ShortAndConcise
	{
		get => IsInDesignMode ? true : field;
		set => OnPropertyChanged(ref field, value);
	} = false;

	public float Temperature
	{
		get => IsInDesignMode ? 1.0F : field;
		set => OnPropertyChanged(ref field, value);
	} = 1.0F;

	public int? MaxTokens
	{
		get => IsInDesignMode ? 100 : field;
		set => OnPropertyChanged(ref field, value);
	} = null;

	public bool RequiredFieldsErrorIsVisible
	{
		get => IsInDesignMode ? true : field;
		set => OnPropertyChanged(ref field, value);
	} = false;
	#endregion

	#region "Commands"
	public AsyncCommand OnCreateCommand => new AsyncCommand(OnCreateCommandAsync);
	public AsyncCommand OnCancelCommand => new AsyncCommand(OnCancelCommandAsync);
	#endregion

	#region "Private Methods"
	public Task OnCreateCommandAsync()
	{
		this.RequiredFieldsErrorIsVisible = false;

		if (this.AssistantName.IsEmpty())
		{
			this.RequiredFieldsErrorIsVisible = true;
			return Task.CompletedTask;
		}

		RequestClose?.Invoke(true);
        return Task.CompletedTask;
    }

	public Task OnCancelCommandAsync()
	{
		RequestClose?.Invoke(false);
		return Task.CompletedTask;
	}
	#endregion
}
