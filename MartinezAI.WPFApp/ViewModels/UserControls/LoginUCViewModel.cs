using MartinezAI.Data;
using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Tools;
using Microsoft.EntityFrameworkCore;

namespace MartinezAI.WPFApp.ViewModels.UserControls;

public class LoginUCViewModel : BaseViewModel
{
	#region "Member Variables"
	readonly StoredProcedures _storedProcs = null!;
	readonly IHashingService _hashingService = null!;
	readonly IUserService _userService = null!;
	#endregion

	#region "Constructor"
	public LoginUCViewModel() { }
	public LoginUCViewModel(
		IServiceProvider serviceProvider,
		StoredProcedures storedProcs,
		IHashingService hashingService,
		IUserService userService) : base(serviceProvider)
	{
		_storedProcs = storedProcs;
		_hashingService = hashingService;
		_userService = userService;

		if (Properties.Settings.Default.SavedUserName != String.Empty)
		{
			this.Email = Properties.Settings.Default.SavedUserName;
			this.SaveUserName = true;
		}
	}
	#endregion

	#region "Public Properties"
	public Func<UserView, Task> onUserAuthentication = null!;
	#endregion

	#region "Form Properties"
	public string Email
	{
		get => IsInDesignMode ? "[EMAIL]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;

	public bool SaveUserName
	{
		get => IsInDesignMode ? true : field;
		set => OnPropertyChanged(ref field, value);
	} = false;

	public bool InvalidLoginIsVisible
	{
		get => IsInDesignMode ? false : field;
		set => OnPropertyChanged(ref field, value);
	} = false;
	#endregion

	#region "Commands"
	public AsyncCommand<string> SignInCommand => new AsyncCommand<string>(OnSignInCommandAsync, (s) => base.IsBusy == false);
	#endregion

	#region "Private Methods"
	private async Task OnSignInCommandAsync(string password)
	{
		try
		{
            this.InvalidLoginIsVisible = false;
			base.IsBusy = true;

            //--Validate required fields.
            if (this.Email.IsEmpty() || password.IsEmpty())
            {
                this.InvalidLoginIsVisible = true;
                return;
            }

            //--Get attempted user hashed password from database.
            string? dbHashedPassword = await _storedProcs.GetUserPasswordAsync(this.Email.Trim());
            if (dbHashedPassword == null)
            {
                this.InvalidLoginIsVisible = true;
                return;
            }

            //--Hash user input to verify user.
            bool isVerified = _hashingService.Verify($"{this.Email.Trim()}|{password.Trim()}", dbHashedPassword);
            if (isVerified == false)
            {
                this.InvalidLoginIsVisible = true;
                return;
            }

            //--User is valid, get user view and let the main view model know.
            UserView authenticatedUser = await _userService.GetAuthenticatedUserAsync(this.Email.Clean()!);

            //--Check if user wants to save the user name.
            if (this.SaveUserName)
            { Properties.Settings.Default.SavedUserName = this.Email.Trim(); }
            else
            { Properties.Settings.Default.SavedUserName = String.Empty; }
            Properties.Settings.Default.Save();

            await onUserAuthentication(authenticatedUser);
        }
		catch (Exception ex) { await base.OnErrorAsync(ex); }
		finally
		{
			this.IsBusy = false;
		}
	}
	#endregion
}