using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Enums;
using OpenAI.Assistants;

namespace MartinezAI.WPFApp.Interfaces;

public interface IDialogService
{
    #region "System Dialogs"
    Task ShowMessageBoxDialogAsync(
    string message,
    MessageBoxIconFlag iconFlag = MessageBoxIconFlag.Info);
    Task<bool> ShowPromptDialogAsync(
        string message,
        string yesText = "Yes",
        string noText = "No");
    #endregion

    #region "Open AI Assistant Dialogs"
    Task<AssistantCreationOptions?> ShowCreateAssistantDialogAsync();
    Task<AssistantModificationOptions?> ShowUpdateAssistantDialogAsync(
        Assistant assistant);
    Task<string?> ShowNewThreadDialogAsync();
    #endregion

    #region "User Dialogs"
    Task<bool> ShowChangePasswordDialogAsync();
    Task<bool> ShowCreateUserDialogAsync();
    #endregion
}
