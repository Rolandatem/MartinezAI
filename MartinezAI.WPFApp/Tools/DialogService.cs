using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Enums;
using MartinezAI.WPFApp.Forms.Dialogs;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Assistants;

namespace MartinezAI.WPFApp.Tools;

internal class DialogService(
    IServiceProvider _serviceProvider,
    ISystemData _systemData) : IDialogService
{
    #region "System Dialogs"
    public Task ShowMessageBoxDialogAsync(
        string message,
        MessageBoxIconFlag iconFlag = MessageBoxIconFlag.Info)
    {
        MessageBoxDialog dialog = _serviceProvider.GetRequiredService<MessageBoxDialog>();
        dialog.Owner = _systemData.MainViewWindow;

        MessageBoxDialogViewModel vm = (MessageBoxDialogViewModel)dialog.DataContext;
        vm.Message = message;
        vm.IconFlag = iconFlag;

        dialog.ShowDialog();

        return Task.CompletedTask;
    }

    public Task<bool> ShowPromptDialogAsync(
        string message,
        string yesText = "Yes",
        string NoText = "No")
    {
        PromptDialog dialog = _serviceProvider.GetRequiredService<PromptDialog>();
        dialog.Owner = _systemData.MainViewWindow;

        PromptDialogViewModel vm = (PromptDialogViewModel)dialog.DataContext;
        vm.PromptMessage = message;
        vm.YesButtonText = yesText;
        vm.NoButtonText = NoText;

        bool? result = dialog.ShowDialog();

        return Task.FromResult(result ?? false);
    }
    #endregion

    #region "Open AI Assistant Dialogs"
    public Task<AssistantCreationOptions?> ShowCreateAssistantDialogAsync()
    {
        UpsertAssistantDialog dialog = _serviceProvider.GetRequiredService<UpsertAssistantDialog>();
        dialog.Owner = _systemData.MainViewWindow;

        UpsertAssistantDialogViewModel vm = (UpsertAssistantDialogViewModel)dialog.DataContext;

        bool? result = dialog.ShowDialog();
        if (result == true)
        {
            return Task.FromResult<AssistantCreationOptions?>(new AssistantCreationOptions()
            {
                Name = vm.AssistantName,
                Instructions = vm.ShortAndConcise
                    ? $"{vm.SystemMessage}{Environment.NewLine}Keep responses short and concise. Be token cost efficient."
                    : vm.SystemMessage,
                Temperature = vm.Temperature
            });
        }

        return Task.FromResult<AssistantCreationOptions?>(null);
    }

    public Task<AssistantModificationOptions?> ShowUpdateAssistantDialogAsync(
        Assistant assistant)
    {
        UpsertAssistantDialog dialog = _serviceProvider.GetRequiredService<UpsertAssistantDialog>();
        dialog.Owner = _systemData.MainViewWindow;

        UpsertAssistantDialogViewModel vm = (UpsertAssistantDialogViewModel)dialog.DataContext;
        vm.DialogTitle = "Edit Assistant";
        vm.AssistantName = assistant.Name;
        vm.SystemMessage = assistant.Instructions;
        vm.Temperature = assistant.Temperature ?? 1.0F;

        bool? result = dialog.ShowDialog();
        if (result == true)
        {
            return Task.FromResult<AssistantModificationOptions?>(new AssistantModificationOptions()
            {
                Name = vm.AssistantName,
                Instructions = vm.ShortAndConcise
                    ? $"{vm.SystemMessage}{Environment.NewLine}Keep responses short and concise. Be token cost efficient."
                    : vm.SystemMessage,
                Temperature = vm.Temperature
            });
        }

        return Task.FromResult<AssistantModificationOptions?>(null); ;
    }

    public Task<string?> ShowNewThreadDialogAsync()
    {
        NewThreadDialog dialog = _serviceProvider.GetRequiredService<NewThreadDialog>();
        dialog.Owner = _systemData.MainViewWindow;

        NewThreadDialogViewModel vm = (NewThreadDialogViewModel)dialog.DataContext;

        bool? result = dialog.ShowDialog();
        if (result == true)
        {
            return Task.FromResult<string?>(vm.NewThreadName);
        }

        return Task.FromResult<string?>(null);
    }
    #endregion

    #region "User Dialogs"
    public Task<bool> ShowChangePasswordDialogAsync()
    {
        ChangePasswordDialog dialog = _serviceProvider.GetRequiredService<ChangePasswordDialog>();
        dialog.Owner = _systemData.MainViewWindow;

        bool? result = dialog.ShowDialog();

        return Task.FromResult(result ?? false);
    }
    public Task<bool> ShowCreateUserDialogAsync()
    {
        CreateUserDialog dialog = _serviceProvider.GetRequiredService<CreateUserDialog>();
        dialog.Owner = _systemData.MainViewWindow;

        bool? result = dialog.ShowDialog();

        return Task.FromResult<bool>(result ?? false);
    }
    #endregion
}