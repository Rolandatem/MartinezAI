using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Tools;
using MartinezAI.WPFApp.ViewModels.UserControls;
using Microsoft.Extensions.DependencyInjection;

namespace MartinezAI.WPFApp.ViewModels.Windows;

public class MainWindowViewModel : BaseViewModel
{
    #region "Events"
    public event Action? RequestClose;
    #endregion

    #region "Member Variables"
    readonly IServiceProvider _serviceProvider = null!;
    #endregion

    #region "Public Properties"
    public UserView? AppUser { get; set; } = null;
    #endregion

    #region "Constructor"
    public MainWindowViewModel() { }
    public MainWindowViewModel(
        IServiceProvider serviceProvider)
    {
        if (IsInDesignMode == false)
        {
            _serviceProvider = serviceProvider;
            OnLogoutCommandAsync();
        }

        _serviceProvider = serviceProvider;
    }
    #endregion

    #region "Form Properties"
    public required string ApplicationTitle
    {
        get => IsInDesignMode ? "Martinez AI (DEV)" : field;
        set => OnPropertyChanged(ref field, $"Martinez AI - {value}");
    } = "Martinez AI";

    public required object? CurrentControl
    {
        get => IsInDesignMode ? new LoginUCViewModel() : field;
        set => OnPropertyChanged(ref field, value);
    }

    public required bool FormIsBusy
    {
        get => field;
        set => OnPropertyChanged(ref field, value);
    } = false;
    #endregion

    #region "Commands"
    public AsyncCommand OnCloseApplicationCommand => new AsyncCommand(OnCloseApplicationCommandAsync);
    public AsyncCommand OnLogoutCommand => new AsyncCommand(OnLogoutCommandAsync);
    #endregion

    #region "Private Methods"
    private Task OnCloseApplicationCommandAsync()
    {
        RequestClose?.Invoke();
        return Task.CompletedTask;
    }
    private Task OnUserAuthenticatedAsync(UserView user)
    {
        this.AppUser = user;
        this.ApplicationTitle = $"{user.FirstName} {user.LastName}";
        this.CurrentControl = _serviceProvider.GetRequiredService<WorkspaceUCViewModel>();

        return Task.CompletedTask;
    }
    private Task OnLogoutCommandAsync()
    {
        this.AppUser = null;
        this.ApplicationTitle = String.Empty;
        LoginUCViewModel vm = _serviceProvider.GetRequiredService<LoginUCViewModel>();
        vm.onUserAuthentication = OnUserAuthenticatedAsync;
        this.CurrentControl = vm;

        return Task.CompletedTask;
    }
    #endregion
}
