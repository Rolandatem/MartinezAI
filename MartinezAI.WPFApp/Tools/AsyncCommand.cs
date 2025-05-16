using System.Windows.Input;

namespace MartinezAI.WPFApp.Tools;

public class AsyncCommand : ICommand
{
	#region "Events"
	public event EventHandler CanExecuteChanged
	{
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}
	#endregion

	#region "Member Variables"
	readonly Func<Task> _executeAsync = null;
	readonly Func<bool> _canExecute = null;
	#endregion

	#region "Constructor"
	public AsyncCommand(
		Func<Task> executeAsync,
		Func<bool> canExecute = null)
	{
		_executeAsync = executeAsync;
		_canExecute = canExecute ?? (() => true);
	}
	#endregion

	#region "Public Properties"
	public bool IsRunning { get; private set; } = false;
	public bool CanExecute(object parameter) => _canExecute();
	#endregion

	public async void Execute(object parameter)
	{
		if (this.IsRunning) { return; }

		this.IsRunning = true;
		try
		{
			await _executeAsync();
		} finally
		{
			this.IsRunning = false;
		}
	}
}
