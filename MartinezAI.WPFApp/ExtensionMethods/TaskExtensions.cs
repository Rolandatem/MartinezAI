using MartinezAI.WPFApp.Interfaces;

namespace MartinezAI.WPFApp;

public static class TaskExtensions
{
    public static async void FireAndForgetSafeAsync(
        this Task task,
        IErrorHandler? errorHandler)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            errorHandler?.HandleError(ex);
        }
    }
}
