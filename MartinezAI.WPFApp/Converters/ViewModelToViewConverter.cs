using MartinezAI.WPFApp.Forms.UserControls;
using MartinezAI.WPFApp.Tools;
using MartinezAI.WPFApp.ViewModels;
using MartinezAI.WPFApp.ViewModels.UserControls;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MartinezAI.WPFApp.Converters;

internal class ViewModelToViewConverter : IValueConverter
{
    #region "Private Methods"
    private UserControl? GetViewByService(object value)
    {
        UserControl? uc = value switch
        {
            LoginUCViewModel => ServiceHelper.Services.GetRequiredService<LoginUC>(),
            WorkspaceUCViewModel => ServiceHelper.Services.GetRequiredService<WorkspaceUC>(),
            EditUsersUCViewModel => ServiceHelper.Services.GetRequiredService<EditUsersUC>(),
            AssistantChatUCViewModel => ServiceHelper.Services.GetRequiredService<AssistantChatUC>(),
            ChatLogUCViewModel => ServiceHelper.Services.GetRequiredService<ChatLogUC>(),
            _ => null
        };

        if (uc != null) { uc.DataContext = value; }

        return uc;
    }

    private UserControl? GetViewForDesignMode(object value)
    {
        return value switch
        {
            LoginUCViewModel => new LoginUC(),
            WorkspaceUCViewModel => new WorkspaceUC(),
            EditUsersUCViewModel => new EditUsersUC(),
            AssistantChatUCViewModel => new AssistantChatUC(),
            ChatLogUCViewModel => new ChatLogUC(),
            _ => null
        };
    }
    #endregion

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (BaseViewModel.IsInDesignMode)
        {
            return GetViewForDesignMode(value);
        }
        else
        {
            return GetViewByService(value);
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
