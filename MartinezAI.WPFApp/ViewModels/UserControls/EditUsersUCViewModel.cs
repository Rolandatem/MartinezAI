using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Tools;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace MartinezAI.WPFApp.ViewModels.UserControls;

public class EditUsersUCViewModel : BaseViewModel
{
	#region "Member Variables"
	readonly IUserService _userService = null!;
	#endregion

	#region "Constructors"
	public EditUsersUCViewModel() { }
	public EditUsersUCViewModel(
		IServiceProvider serviceProvider,
		IUserService userService)
		: base(serviceProvider)
	{
		_userService = userService;
	}
	#endregion

	#region "Form Properties"
	public ObservableCollection<User> Users
	{
		get
		{
			if (IsInDesignMode)
			{
				return new ObservableCollection<User>()
				{
					new User()
					{
						Email = "user1@gmail.com",
						FirstName = "First",
						LastName = "User",
						Password = String.Empty,
						IsAdmin = false
					},
					new User()
					{
						Email = "user2@gmail.com",
						FirstName = "Second",
						LastName = "User",
						Password = String.Empty,
						IsAdmin = true
					}
				};
			}
			else { return field; }
		}
		set => OnPropertyChanged(ref field, value);
	} = new ObservableCollection<User>();

	public User? SelectedUser
	{
		get => IsInDesignMode ? this.Users[0] : field;
		set => OnPropertyChanged(ref field, value);
	}
	#endregion

	#region "Commands"
	public AsyncCommand OnRefreshUsersCommand => new AsyncCommand(OnRefreshUsersCommandAsync);
	public AsyncCommand<User> OnToggleIsAdminCommand => new AsyncCommand<User>(OnToggleIsAdminCommandAsync);
	public AsyncCommand<User> OnResetUserPasswordCommand => new AsyncCommand<User>(OnResetUserPasswordCommandAsync);
	public AsyncCommand<User> OnDeleteUserCommand => new AsyncCommand<User>(OnDeleteUserCommandAsync);
	public AsyncCommand OnCreateUserCommand => new AsyncCommand(OnCreateUserCommandAsync);
	#endregion

	#region "Private Methods"
	private async Task OnRefreshUsersCommandAsync()
	{
		try
		{
			base.IsBusy = true;
			this.SelectedUser = null;
            IQueryable<User> userQuery = await _userService.GetAllUsersAsync();
			this.Users = [.. await userQuery
				.OrderBy(user => user.FirstName)
                .ToListAsync()
				.ConfigureAwait(false)];
		}
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex, "Failed to refresh users.");
		}
		finally { base.IsBusy = false; }
	}
	private async Task OnToggleIsAdminCommandAsync(User user)
	{
		try
		{
            if (user == null) { return; }

			user.IsAdmin = !user.IsAdmin;
			await _userService.SaveModifiedUserAsync(user);
        }
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex, "Failed to update user IsAdmin status.");
			user.IsAdmin = !user.IsAdmin;
		}
    }
    private async Task OnResetUserPasswordCommandAsync(User user)
    {
		bool response = await base.DialogService!.ShowPromptDialogAsync($"Are you sure you want to reset {user.FirstName}'s password?");
		if (response)
		{
			base.IsBusy = true;
			try
			{
				string tempPassword = await _userService.ResetUserPasswordAsync(user.Email);
				await base.DialogService!.ShowMessageBoxDialogAsync($"{user.FirstName}'s password reset successfully and saved to your clipboard!");
				Clipboard.SetText(tempPassword);
			}
			catch (Exception ex)
			{
				await base.OnErrorAsync(ex, "Failed to reset user password.");
			}
			finally { base.IsBusy = false; }
		}
    }
    private async Task OnDeleteUserCommandAsync(User user)
    {
		bool response = await base.DialogService!.ShowPromptDialogAsync($"Are you sure you want to delete {user.FirstName}?");
		if (response)
		{
			try
			{
				base.IsBusy = true;
				await _userService.DeleteUserAsync(user.Email);
				await base.DialogService!.ShowMessageBoxDialogAsync($"{user.FirstName} deleted successfully!");
			}
			catch (Exception ex) { await base.OnErrorAsync(ex, "Failed to delete user."); }
			finally { base.IsBusy = false; }
		}
    }
	private async Task OnCreateUserCommandAsync()
	{
		bool created = await base.DialogService!.ShowCreateUserDialogAsync();
		if (created)
		{
			await OnRefreshUsersCommandAsync();
		}
	}
    #endregion
}