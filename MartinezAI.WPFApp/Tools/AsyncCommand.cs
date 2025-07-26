using MartinezAI.WPFApp.Interfaces;
using System.Windows.Input;

namespace MartinezAI.WPFApp.Tools;

public class AsyncCommand : ICommand
{
    #region "Events"
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
    #endregion

    #region "Member Variables"
    readonly Func<Task> _executeAsync = null!;
    readonly Func<bool>? _canExecute = null;
    readonly IErrorHandler? _errorHandler = null;
    #endregion

    #region "Constructor"
    public AsyncCommand(
        Func<Task> executeAsync,
        Func<bool>? canExecute = null,
        IErrorHandler? errorHandler = null)
    {
        _executeAsync = executeAsync;
        _canExecute = canExecute;
        _errorHandler = errorHandler;
    }
    #endregion

    #region "Public Properties"
    public bool IsRunning { get; private set; } = false;
    public bool CanExecute() =>
        this.IsRunning == false &&
        (_canExecute?.Invoke() ?? true);
    #endregion

    public async Task ExecuteAsync()
    {
        if (this.IsRunning) { return; }

        try
        {
            this.IsRunning = true;
            await _executeAsync();
        }
        finally
        {
            this.IsRunning = false;
        }
    }

    #region "ICommand"
    bool ICommand.CanExecute(object? parameter)
    {
        return CanExecute();
    }
    void ICommand.Execute(object? parameter)
    {
        ExecuteAsync().FireAndForgetSafeAsync(_errorHandler);
    }
    #endregion
}

public class AsyncCommand<T> : ICommand
{
    #region "Events"
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
    #endregion

    #region "Member Variables"
    readonly Func<T, Task> _executeAsync = null!;
    readonly Func<T, bool>? _canExecute = null;
    readonly IErrorHandler? _errorHandler = null;
    #endregion

    #region "Constructor"
    public AsyncCommand(
        Func<T, Task> executeAsync,
        Func<T, bool>? canExecute = null,
        IErrorHandler? errorHandler = null)
    {
        _executeAsync = executeAsync;
        _canExecute = canExecute;
        _errorHandler = errorHandler;
    }
    #endregion

    #region "Public Properties"
    public bool IsRunning { get; private set; } = false;
    public bool CanExecute(T parameter) =>
        this.IsRunning == false &&
        (_canExecute?.Invoke(parameter) ?? true);
    #endregion

    public async Task ExecuteAsync(T parameter)
    {
        if (this.CanExecute(parameter) == false) { return; }

        try
        {
            this.IsRunning = true;
            await _executeAsync(parameter);
        }
        finally
        {
            this.IsRunning = false;
        }
    }

    #region "ICommand"
    bool ICommand.CanExecute(object? parameter)
    {
        return this.CanExecute((T)parameter!);
    }
    void ICommand.Execute(object? parameter)
    {
        ExecuteAsync((T)parameter!).FireAndForgetSafeAsync(_errorHandler);
    }
    #endregion
}