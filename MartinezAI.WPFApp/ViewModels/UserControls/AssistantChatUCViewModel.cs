using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Models;
using MartinezAI.WPFApp.Tools;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Assistants;
using System.Collections.ObjectModel;

namespace MartinezAI.WPFApp.ViewModels.UserControls;

public class AssistantChatUCViewModelDesign : BaseViewModel
{
	#region "Design Classes"
	public class DesignAssistant
	{
		public string Name { get; set; }
		public string Instructions { get; set; }
		public float Temperature { get; set; }

		public DesignAssistant(
			string name,
			string instructions,
			float temperature)
		{
			this.Name = name;
			this.Instructions = instructions;
			this.Temperature = temperature;
		}
	}
	#endregion

	#region "Form Properties"
	public DesignAssistant Assistant { get; set; } = new DesignAssistant("Design Assistant", "Design Instructions", 1.0F);
	public ObservableCollection<UserAssistantThread> Threads { get; } = 
		[
			new UserAssistantThread() { ThreadName = "Thread 1", AssistantId = "123", ThreadId = "123"},
			new UserAssistantThread() { ThreadName = "Thread 2", AssistantId = "123", ThreadId = "123"},
			new UserAssistantThread() { ThreadName = "Thread 3", AssistantId = "123", ThreadId = "123"},
		];
	public UserAssistantThread? SelectedThread { get; set; }
	public object ChatLogControl => new ChatLogUCViewModel();
	public string UserInput { get; set; }= "[USER_INPUT]";
	#endregion

	#region "Constructor"
	public AssistantChatUCViewModelDesign()
	{
		this.SelectedThread = this.Threads[0];
	}
	#endregion

	#region "Commands"
	public AsyncCommand OnNewThreadCommand => new AsyncCommand(null!);
	public AsyncCommand OnAddMessageCommand => new AsyncCommand(null!);
	public AsyncCommand OnRunCommand => new AsyncCommand(null!);
	public AsyncCommand OnLoadPreviousMessagesCommand = new AsyncCommand(null!);
	#endregion
}

public class AssistantChatUCViewModel : BaseViewModel
{
	#region "Member Variables"
	readonly IOpenAIService _openAIService = null!;
	readonly IUserService _userService = null!;
	#endregion

	#region "Constructors"
	public AssistantChatUCViewModel() { }
	public AssistantChatUCViewModel(
		IServiceProvider serviceProvider,
		IOpenAIService openAIService,
		IUserService userService) : base(serviceProvider) 
	{
		_openAIService = openAIService;
		_userService = userService;
	}
	#endregion

	#region "Private Properties"
	private Dictionary<string, ObservableCollection<ChatLogMessage>> ThreadChatHistory = new Dictionary<string, ObservableCollection<ChatLogMessage>>();
	#endregion

	#region "Form Properties"
	public Assistant? Assistant
	{
		get => field;
		set => OnPropertyChanged(ref field, value);
	} = null;
	public ObservableCollection<UserAssistantThread> Threads
	{
		get => field;
		set => OnPropertyChanged(ref field, value);
	} = new ObservableCollection<UserAssistantThread>();
	public UserAssistantThread? SelectedThread
	{
		get => field;
		set
		{
			//--Save current thread chat history.
			if (field != null)
			{
				this.ThreadChatHistory[field.ThreadId] = this.ChatLogControl!.ChatLogMessages;
			}

            OnPropertyChanged(ref field, value);
			this.ChatLogControl = base.Services!.GetRequiredService<ChatLogUCViewModel>();

			//--Load existing chat
			if (value != null && this.ThreadChatHistory.TryGetValue(value.ThreadId, out ObservableCollection<ChatLogMessage>? cachedMessages))
			{
				this.ChatLogControl!.ChatLogMessages = cachedMessages;
			}
			else
			{
				this.ChatLogControl!.ChatLogMessages = new ObservableCollection<ChatLogMessage>();
			}
        }
	}
	public string UserInput
	{
		get => field;
		set => OnPropertyChanged(ref field, value);
	} = String.Empty;
	public ChatLogUCViewModel? ChatLogControl
	{
		get => field;
		set => OnPropertyChanged(ref field, value);
	} = null;
	#endregion

	#region "Commands"
	public AsyncCommand OnNewThreadCommand => new AsyncCommand(OnNewThreadCommandAsync);
	public AsyncCommand OnDeleteThreadCommand => new AsyncCommand(OnDeleteThreadCommandAsync);
	public AsyncCommand OnAddMessageCommand => new AsyncCommand(OnAddMessageCommandAsync, () => this.SelectedThread != null);
	public AsyncCommand OnRunCommand => new AsyncCommand(OnRunCommand2Async, () => this.SelectedThread != null);
	public AsyncCommand OnLoadPreviousMessagesCommand => new AsyncCommand(OnLoadPreviousMessagesCommandAsync);
    #endregion

    #region "Public Methods"
    public async Task OnChatLoadedAsync()
    {
		try
		{
			base.IsBusy = true;
			if (this.Assistant != null)
			{
				this.Threads = new ObservableCollection<UserAssistantThread>(
					await _userService.GetAssistantThreadsAsync(this.Assistant.Id));
			}
		}
		catch (Exception ex) { await base.OnErrorAsync(ex, "Failed to load assistant threads."); }
		finally { base.IsBusy = false; }
    }
    #endregion

    #region "Private Methods"
    private async Task OnNewThreadCommandAsync()
	{
		base.IsBusy = true;
		try
		{
			string? newThreadName = await base.DialogService!.ShowNewThreadDialogAsync();
			if (newThreadName!.Exists())
			{
				AssistantThread newThread = await _openAIService.CreateNewThreadAsync();
				UserAssistantThread newLocalThread = await _userService.CreateNewThreadAsync(
					newThreadName!,
					this.Assistant!.Id,
					newThread.Id);

				this.Threads.Add(newLocalThread);
			}

		}
		catch (Exception ex) { await base.OnErrorAsync(ex); }
		finally { base.IsBusy = false; }
	}
	private async Task OnDeleteThreadCommandAsync()
	{
		if (this.SelectedThread != null)
		{
			bool result = await base.DialogService!.ShowPromptDialogAsync($"Are you sure you want to delete the thread [{this.SelectedThread.ThreadName}]?");

			if (result)
			{
				try
				{
					base.IsBusy = true;
					await _openAIService.DeleteThreadAsync(this.SelectedThread.ThreadId);

					await base.DialogService!.ShowMessageBoxDialogAsync("Open AI Thread deleted successfully!", Enums.MessageBoxIconFlag.Info);
				}
				catch (Exception ex) { await base.OnErrorAsync(ex); }
				finally { base.IsBusy = false; }

				try
				{
                    await _userService.DeleteThreadAsync(this.SelectedThread.ThreadId);

                    this.Threads.Remove(this.SelectedThread);
                    this.SelectedThread = null;

					await base.DialogService!.ShowMessageBoxDialogAsync("DB Thread deleted successfully!", Enums.MessageBoxIconFlag.Info);
                }
				catch (Exception ex) { await base.OnErrorAsync(ex); }
				finally { base.IsBusy = false; }
			}
        }
	}
	private async Task OnAddMessageCommandAsync()
	{
		if (this.UserInput.Exists())
		{
			this.ChatLogControl!.ChatLogMessages.Add(new ChatLogMessage()
			{
				Owner = base.CurrentUser!.FirstName,
				Content = this.UserInput
			});

            await _openAIService.CreateThreadMessageAsync(
				this.SelectedThread!.ThreadId,
				this.UserInput);

            this.UserInput = String.Empty;
        }
	}
	private async Task OnRunCommandAsync()
	{
		try
		{
			base.IsBusy = true;
			if (this.UserInput.Exists())
			{
				await OnAddMessageCommandAsync();
			}

			(List<string> messages, string lastMessageId) assistantMessages = await _openAIService.RunThreadAsync(
				this.SelectedThread!.ThreadId,
				this.Assistant!.Id,
				this.SelectedThread!.LastMessageId);

			this.SelectedThread!.LastMessageId = assistantMessages.lastMessageId;
			await _userService.UpdateLastMessageIdAsync(
				this.SelectedThread!.Id,
				this.SelectedThread!.LastMessageId);
			foreach (string message in assistantMessages.messages)
			{
				this.ChatLogControl!.ChatLogMessages.Add(new ChatLogMessage()
				{
					Owner = this.Assistant.Name,
					Content = message
				});
			}
		}
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex);
		}
		finally { base.IsBusy = false; }
	}
	private async Task OnRunCommand2Async()
	{
		try
		{
			if (this.UserInput.Exists())
			{
				await OnAddMessageCommandAsync();
			}

			ChatLogMessage assistantMessage = new ChatLogMessage()
			{
				Owner = this.Assistant!.Name,
				Content = String.Empty
			};
			this.ChatLogControl!.ChatLogMessages.Add(assistantMessage);

			await _openAIService.RunThreadStreamingAsync(
				this.SelectedThread!.ThreadId,
				this.Assistant!.Id,
				this.SelectedThread!.LastMessageId,
				assistantMessage);
			await _userService.UpdateLastMessageIdAsync(
				this.SelectedThread!.Id,
				this.SelectedThread!.LastMessageId!);
		}
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex);
		}
	}
	private async Task OnLoadPreviousMessagesCommandAsync()
	{
		try
		{
			base.IsBusy = true;
			List<ChatLogMessage> previousMessages = await _openAIService.GetPreviousMessagesAsync(this.SelectedThread!.ThreadId);

			//--Fix owners
			foreach (ChatLogMessage msg in previousMessages)
			{
				if (msg.Owner == "User") { msg.Owner = base.CurrentUser!.FirstName; }
				else { msg.Owner = this.Assistant!.Name; }
			}

			this.ChatLogControl!.ChatLogMessages = new ObservableCollection<ChatLogMessage>(previousMessages);
		}
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex);
		}
		finally { base.IsBusy = false; }
	}
    #endregion

    //public async Task AddStreamingMessageAsync(string owner, IAsyncEnumerable<string> contentStream)
    //{
    //    var message = new ChatLogMessage { Owner = owner };
    //    ChatLogMessages.Add(message);

    //    await foreach (var contentPart in contentStream)
    //    {
    //        message.Content += contentPart;
    //        // Trigger visual update here if necessary
    //    }
    //}
}
