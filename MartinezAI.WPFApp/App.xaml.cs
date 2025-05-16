using MartinezAI.WPFApp.Settings;
using MartinezAI.WPFApp.ViewModels;
using MartinezAI.WPFApp.Windows;
using MartinezAI.WPFApp.Windows.Dialogs;
using Microsoft.Extensions.Configuration;
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

        MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}

public static class ServicecCollectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        //--Configuration
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("settings/appsettings.json", false, true)
            .Build();
        services.AddSingleton<IConfiguration>(config);

        //--Configure options
        services
            .Configure<SystemSettings>(config.GetSection("SystemSettings"));

        //--Windows
        services
            .AddSingleton<MainWindow>();

        //--Dialogs
        services
            .AddTransient<CreateChatDialog>();

        //--ViewModels
        services
            //--Windows
            .AddTransient<IMainWindowViewModel, MainWindowViewModel>()
            
            //--Dialogs
            .AddTransient<ICreateChatDialogViewModel, CreateChatDialogViewModel>();
    }
}