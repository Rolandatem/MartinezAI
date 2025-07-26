using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Tools;

namespace MartinezAI.WPFApp.ViewModels.Dialogs;

public class ChangePasswordDialogViewModel : BaseViewModel
{
	#region "Events"
	public event Action<bool>? RequestClose;
	#endregion

	#region "Member Variables"
	readonly IUserService _userService = null!;
	#endregion

	#region "Constructors"
	public ChangePasswordDialogViewModel() { }
	public ChangePasswordDialogViewModel(
		IServiceProvider serviceProvider,
		IUserService userService)
		: base(serviceProvider)
	{
		_userService = userService;
	}
	#endregion

	#region "Commands"
	public AsyncCommand OnCancelCommand => new AsyncCommand(OnCancelCommandAsync);
	#endregion

	#region "Private Methods"
	public async Task OnUpdateCommandAsync(string newPassword, string confirmPassword)
	{
		try
		{
            //--Validate
			if (newPassword == null || confirmPassword == null)
			{
				await base.DialogService!.ShowMessageBoxDialogAsync("New and Confirm Passwords are required!", Enums.MessageBoxIconFlag.Error);
				return;
			}

            if (newPassword.Trim() != confirmPassword.Trim())
            {
                await base.DialogService!.ShowMessageBoxDialogAsync("Passwords must match!", Enums.MessageBoxIconFlag.Error);
                return;
            }

			//--Get User
			User? user = await _userService.GetUserByEmailAsync(CurrentUser!.Email);
			if (user == null)
			{
				await base.DialogService!.ShowMessageBoxDialogAsync($"Could not find user: {base.CurrentUser!.Email} in the database.", Enums.MessageBoxIconFlag.Error);
				return;
			}

			//--Update User
			await _userService.UpdateUserPasswordAsync(base.CurrentUser!.Email, newPassword);

			RequestClose?.Invoke(true);
        }
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex, "Failed to update password");
		}

    }
	public Task OnCancelCommandAsync()
	{
		RequestClose?.Invoke(false);
		return Task.CompletedTask;
	}
	#endregion
}
