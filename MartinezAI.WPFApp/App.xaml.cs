using CefSharp;
using CefSharp.Wpf;
using MartinezAI.Data;
using MartinezAI.WPFApp.Forms.Dialogs;
using MartinezAI.WPFApp.Forms.UserControls;
using MartinezAI.WPFApp.Forms.Windows;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Tools;
using MartinezAI.WPFApp.ViewModels.Dialogs;
using MartinezAI.WPFApp.ViewModels.UserControls;
using MartinezAI.WPFApp.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MartinezAI.WPFApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureServices();

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        ServiceHelper.Services = serviceProvider;

        MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        CefSettings settings = new CefSettings();
        settings.LogSeverity = CefSharp.LogSeverity.Info;
        settings.CachePath = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            "CefSharpApp_",
            Environment.ProcessId.ToString());
        Cef.Initialize(settings);
    }
}

public static class ServiceCollectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        //--Configuration
        //IConfiguration config = new ConfigurationBuilder()
        //    .AddJsonFile("settings/appsettings.json")
        //    .Build();
        //services.AddSingleton<IConfiguration>(config);

        ////--Configure options
        //services
        //    .Configure<SystemSettings>(config.GetSection("SystemSettings"));

        //--DbContext
        services.AddDbContext<MartinezAIDbContext>(ServiceLifetime.Transient);
        //----Stored procedures
        services.AddTransient<StoredProcedures>();

        //--Custom services
        services
            .AddSingleton<ISystemData, SystemData>()
            .AddSingleton<IHashingService, HashingService>()
            .AddTransient<IDialogService, DialogService>()
            .AddTransient<IOpenAIService, OpenAIService>()
            .AddTransient<IUserService, UserService>()
            .AddSingleton<IMarkdownToHtmlConverter, MarkdownToHtmlConverter>();

        //--Forms & ViewModels
        services
            .AddTransient<MessageBoxDialog>()
            .AddTransient<MessageBoxDialogViewModel>()

            .AddTransient<PromptDialog>()
            .AddTransient<PromptDialogViewModel>()

            .AddSingleton<MainWindow>()
            .AddSingleton<MainWindowViewModel>()

            .AddTransient<LoginUC>()
            .AddTransient<LoginUCViewModel>()
            
            .AddTransient<WorkspaceUC>()
            .AddTransient<WorkspaceUCViewModel>()
            
            .AddTransient<UpsertAssistantDialog>()
            .AddTransient<UpsertAssistantDialogViewModel>()
            
            .AddTransient<ChangePasswordDialog>()
            .AddTransient<ChangePasswordDialogViewModel>()
            
            .AddTransient<EditUsersUC>()
            .AddTransient<EditUsersUCViewModel>()
            
            .AddTransient<CreateUserDialog>()
            .AddTransient<CreateUserDialogViewModel>()
            
            .AddTransient<AssistantChatUC>()
            .AddTransient<AssistantChatUCViewModel>()
            
            .AddTransient<NewThreadDialog>()
            .AddTransient<NewThreadDialogViewModel>()
            
            .AddTransient<ChatLogUC>()
            .AddTransient<ChatLogUCViewModel>();
    }
}
