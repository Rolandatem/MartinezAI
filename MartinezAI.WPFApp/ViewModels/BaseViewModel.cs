using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Tools;
using MartinezAI.WPFApp.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace MartinezAI.WPFApp.ViewModels;

public abstract class BaseViewModel : NotifyableClass
{
    #region "Member Variables"
    readonly IServiceProvider? _serviceProvider = null;
    IDialogService? _dialogService = null;
    MainWindowViewModel? _mainWindowViewModel = null;
    #endregion

    #region "Constructors"
    internal BaseViewModel() { }
    internal BaseViewModel(
        IServiceProvider? serviceProvider = null)
    {
        _serviceProvider = serviceProvider;
    }
    #endregion

    #region "Public Properties"
    public static bool IsInDesignMode =>
        DesignerProperties.GetIsInDesignMode(new DependencyObject());

    public IServiceProvider? Services => _serviceProvider;
    public IDialogService? DialogService
    {
        get { return _dialogService ??= _serviceProvider!.GetRequiredService<IDialogService>(); }
        set { _dialogService = value; }
    }

    public bool IsBusy
    {
        get => this.MainWindowVM.FormIsBusy;
        set => this.MainWindowVM.FormIsBusy = value;
    }

    public UserView? CurrentUser
    {
        get => this.MainWindowVM.AppUser;
    }
    #endregion

    #region "Private Methods"
    private MainWindowViewModel MainWindowVM
    {
        get => _mainWindowViewModel ??= _serviceProvider!.GetRequiredService<MainWindowViewModel>();
        set => _mainWindowViewModel = value;
    }
    #endregion

    #region "Public Methods"
    public async Task OnErrorAsync(
        Exception ex,
        string message = "")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(message);
        sb.AppendLine();

        for (var e = ex; e != null; e = e.InnerException)
        {
            sb.AppendLine("-------------");
            sb.AppendLine(e.Message);
        }

        string result = sb.ToString();

        await this.DialogService!.ShowMessageBoxDialogAsync(result, Enums.MessageBoxIconFlag.Error);
    }
    #endregion
}