using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Tools;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Assistants;
using System.Collections.ObjectModel;

namespace MartinezAI.WPFApp.ViewModels.UserControls;

public class WorkspaceUCViewModelDesign: BaseViewModel
{
	#region "Mock Classes"
	public class DesignAssistant
	{
		public string Name { get; set; }
		public DesignAssistant(string name)
		{
			this.Name = name;
		}
	}
	#endregion

	#region "Form Properties"
	public ObservableCollection<DesignAssistant> Assistants => new ObservableCollection<DesignAssistant>()
    {
        new DesignAssistant("Assistant A"),
        new DesignAssistant("Assistant B"),
        new DesignAssistant("Assistant C"),
        new DesignAssistant("Assistant D"),
        new DesignAssistant("Assistant E")
    };

	public DesignAssistant SelectedAssistant
	{
		get => this.Assistants[0];
		set => this.Assistants[0] = value;
    }
	#endregion
}

public class WorkspaceUCViewModel(
	IServiceProvider _serviceProvider,
	IOpenAIService _openAIService) 
	: BaseViewModel(_serviceProvider)
{

	#region "Form Properties"
	public ObservableCollection<Assistant> Assistants
	{
		get => field;
		set => OnPropertyChanged(ref field, value);
	} = new ObservableCollection<Assistant>();

	public Assistant? SelectedAssistant
	{
		get => field;
		set
		{
            OnPropertyChanged(ref field, value);
			
			AssistantChatUCViewModel assistantChatVM = base.Services!.GetRequiredService<AssistantChatUCViewModel>();
			assistantChatVM.Assistant = value;
			this.WorkspaceContent = assistantChatVM;
        }
	} = null;

	public required object? WorkspaceContent
	{
		get => field;
		set => OnPropertyChanged(ref field, value);
	}
	#endregion

	#region "Commands"
	public AsyncCommand OnLoadAssistantsCommand => new AsyncCommand(OnLoadAssistantsCommandAsync);
	public AsyncCommand OnCreateNewAssistantCommand => new AsyncCommand(OnCreateNewAssistantCommandAsync);
	public AsyncCommand<Assistant> OnEditAssistantCommand => new AsyncCommand<Assistant>(OnEditAssistantCommandAsync);
	public AsyncCommand<Assistant> OnDeleteAssistantCommand => new AsyncCommand<Assistant>(OnDeleteAssistantCommandAsync);
	public AsyncCommand OnEditUsersCommand => new AsyncCommand(OnEditUsersCommandAsync);
	public AsyncCommand OnChangePasswordCommand => new AsyncCommand(OnChangePasswordCommandAsync);
	#endregion

	#region "Private Methods"
	public async Task OnCreateNewAssistantCommandAsync()
    {
		try
		{
            AssistantCreationOptions? assistantOptions = await base.DialogService!.ShowCreateAssistantDialogAsync();

			if (assistantOptions == null) { return; }

			base.IsBusy = true;
            assistantOptions.Metadata.Add("user", base.CurrentUser!.Email);
            Assistant newAssistant = await _openAIService.CreateAssistantAsync(assistantOptions);

			this.Assistants.Add(newAssistant);

        }
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex, "Failed to create open new Assistant");
		}
		finally
		{
			this.IsBusy = false;
		}
    }
	public async Task OnLoadAssistantsCommandAsync()
	{
		try
		{
            base.IsBusy = true;
            IEnumerable<Assistant> preList = await _openAIService.GetAssistantsAsync();
			this.Assistants = [.. preList
				.Where(assistant =>
					assistant.Metadata != null &&
					assistant.Metadata.TryGetValue("user", out string? value) &&
					value == base.CurrentUser!.Email)];
        }
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex);
		}
		finally
		{
			this.IsBusy = false;
		}
	}
	public async Task OnEditAssistantCommandAsync(Assistant assistant)
	{
		try
		{
            this.SelectedAssistant = assistant;

			AssistantModificationOptions? options = await base.DialogService!.ShowUpdateAssistantDialogAsync(assistant);

			if (options == null) { return; }

			base.IsBusy = true;
			Assistant updatedAssistant = await _openAIService.UpdateAssistantAsync(assistant.Id, options);

			int idx = this.Assistants.IndexOf(assistant);
			this.Assistants[idx] = updatedAssistant;

			this.SelectedAssistant = updatedAssistant;
        }
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex, "Failed to edit assistant.");
		}
		finally { this.IsBusy = false; }
	}
	public async Task OnDeleteAssistantCommandAsync(Assistant assistant)
	{
		try
		{
            this.SelectedAssistant = assistant;

            bool result = await base.DialogService!.ShowPromptDialogAsync(
				$"Are you sure you want to delete '{assistant.Name}'?");

			if (result == false) { return; }

			base.IsBusy = true;
            bool response = await _openAIService.DeleteAssistantAsync(assistant.Id);

			if (response)
			{
				this.Assistants.Remove(assistant);
			}
			else
			{
				await base.DialogService.ShowMessageBoxDialogAsync("Open AI did not return a positive respose for deleting the assistant.", Enums.MessageBoxIconFlag.Warning);
			}
        }
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex, "Failed to delete assistnant.");
		}
		finally { base.IsBusy = false; }
	}
	public Task OnEditUsersCommandAsync()
	{
		this.WorkspaceContent = base.Services!.GetRequiredService<EditUsersUCViewModel>();
		return Task.CompletedTask;
	}
	public async Task OnChangePasswordCommandAsync()
	{
		try
		{
			bool result = await base.DialogService!.ShowChangePasswordDialogAsync();

			if (result)
			{
				await base.DialogService!.ShowMessageBoxDialogAsync("Password Updated Successfully!", Enums.MessageBoxIconFlag.Info);
			}
		}
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex, "Failed to update password.");
		}
	}
	#endregion
}
