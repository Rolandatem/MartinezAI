using MartinezAI.Data;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Tools;

namespace MartinezAI.WPFApp.ViewModels.Dialogs;

public class CreateUserDialogViewModel : BaseViewModel
{
	#region "Events"
	public event Action<bool>? RequestClose;
	#endregion

	#region "Member Variables"
	readonly IHashingService _hashingService = null!;
	readonly IUserService _userService = null!;
	#endregion

	#region "Constructors"
	public CreateUserDialogViewModel() { }
	public CreateUserDialogViewModel(
		IServiceProvider serviceProvider,
		IHashingService hashingService,
		IUserService userService)
		: base(serviceProvider)
	{
		_hashingService = hashingService;
		_userService = userService;
	}
	#endregion

	#region "Form Properties"
	public string FirstName
	{
		get => IsInDesignMode ? "[FIRST_NAME]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;
	public string LastName
	{
		get => IsInDesignMode ? "[LAST_NAME]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;
	public string Email
	{
		get => IsInDesignMode ? "[EMAIL]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;
	public string Password
	{
		get => IsInDesignMode ? "[PASSWORD]" : field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;
	public bool IsAdmin
	{
		get => IsInDesignMode ? true : field;
		set => OnPropertyChanged(ref field, value);
	} = false;

	public bool FirstNameRequiredIsVisible
	{
		get => IsInDesignMode ? true : field;
		set => OnPropertyChanged(ref field, value);
	} = false;
	public bool LastNameRequiredIsVisible
	{
		get => IsInDesignMode ? true : field;
		set => OnPropertyChanged(ref field, value);
	} = false;
	public bool EmailRequiredIsVisible
	{
		get => IsInDesignMode ? true : field;
		set => OnPropertyChanged(ref field, value);
	} = false;
	public bool PasswordRequiredIsVisible
	{
		get => IsInDesignMode ? true : field;
		set => OnPropertyChanged(ref field, value);
	} = false;
	#endregion

	#region "Commands"
	public AsyncCommand OnCreateUserCommand => new AsyncCommand(OnCreateUserCommandAsync);
	public AsyncCommand OnCancelCommand => new AsyncCommand(OnCancelCommandAsync);
	public AsyncCommand OnGeneratePasswordCommand => new AsyncCommand(OnGeneratePasswordCommandAsync);
	#endregion

	#region "Private Methods"
	public async Task OnCreateUserCommandAsync()
	{
		try
		{
			base.IsBusy = true;

            //--Reset Validation
            this.FirstNameRequiredIsVisible = false;
            this.LastNameRequiredIsVisible = false;
            this.EmailRequiredIsVisible = false;
            this.PasswordRequiredIsVisible = false;

            //--Validate
            if (this.FirstName.IsEmpty()) { this.FirstNameRequiredIsVisible = true; }
            if (this.LastName.IsEmpty()) { this.LastNameRequiredIsVisible = true; }
            if (this.Email.IsEmpty()) { this.EmailRequiredIsVisible = true; }
            if (this.Password.IsEmpty()) { this.PasswordRequiredIsVisible = true; }
            if (this.FirstNameRequiredIsVisible ||
                this.LastNameRequiredIsVisible ||
                this.EmailRequiredIsVisible ||
                this.PasswordRequiredIsVisible) { return; }

			//--Attempt Create User
			await _userService.CreateUserAsync(
				this.Email.Clean()!,
				this.FirstName.Clean()!,
				this.LastName.Clean()!,
				this.Password.Clean()!,
				this.IsAdmin);

			RequestClose?.Invoke(true);
        }
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex, "Failed to create user.");
		}
		finally { base.IsBusy = false; }

    }
	public Task OnCancelCommandAsync()
	{
		RequestClose?.Invoke(false);
		return Task.CompletedTask;
	}
	public Task OnGeneratePasswordCommandAsync()
	{
		this.Password = _hashingService.GenerateRandomPrehashPassword(20);
		return Task.CompletedTask;
    }
	#endregion
}
