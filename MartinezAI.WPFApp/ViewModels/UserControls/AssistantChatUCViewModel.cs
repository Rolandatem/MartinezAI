using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Models;
using MartinezAI.WPFApp.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML.Tokenizers;
using OpenAI.Assistants;
using System.Collections.ObjectModel;
using System.Text;
using System.Transactions;
using System.Windows.Media;

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
	public Dictionary<string, ThreadHistory> ThreadChatHistory = new Dictionary<string, ThreadHistory>();
	public DesignAssistant Assistant { get; set; } = new DesignAssistant("Design Assistant", "Design Instructions", 1.0F);
	public ObservableCollection<UserAssistantThread> Threads { get; } = 
		[
			new UserAssistantThread() { ThreadName = "Thread 1", AssistantId = "123", ThreadId = "123"},
			new UserAssistantThread() { ThreadName = "Thread 2", AssistantId = "123", ThreadId = "1234"},
			new UserAssistantThread() { ThreadName = "Thread 3", AssistantId = "123", ThreadId = "1235"},
		];
	public UserAssistantThread? SelectedThread { get; set; }
	public ThreadHistory? SelectedThreadHistory { get; set; }
	public object ChatLogControl => new ChatLogUCViewModel();
	public string UserInput { get; set; }= "[USER_INPUT]";
	public double TokensRatio => 0.6F;
	public Brush TokenUsageBrush => (Brush)App.Current.Resources["MainFontForegroundBrush"];
	public int MaxTPM => 30000;
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
	public AsyncCommand OnSummarizeThreadMessagesCommand = new AsyncCommand(null!);
	#endregion
}

public class AssistantChatUCViewModel : BaseViewModel
{
	#region "Member Variables"
	readonly IOpenAIService _openAIService = null!;
	readonly IUserService _userService = null!;
	readonly ISystemData _systemData = null!;
	#endregion

	#region "Constructors"
	public AssistantChatUCViewModel() { }
	public AssistantChatUCViewModel(
		IServiceProvider serviceProvider,
		IOpenAIService openAIService,
		IUserService userService,
		ISystemData systemData) : base(serviceProvider) 
	{
		_openAIService = openAIService;
		_userService = userService;
		_systemData = systemData;
	}
    #endregion

    #region "Form Properties"
    public Dictionary<string, ThreadHistory> ThreadChatHistory = new Dictionary<string, ThreadHistory>();
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
				if (this.ThreadChatHistory.ContainsKey(field.ThreadId) == false)
				{
					this.ThreadChatHistory[field.ThreadId] = new ThreadHistory();
				}

				this.ThreadChatHistory[field.ThreadId].Messages = this.ChatLogControl!.ChatLogMessages;
			}

            OnPropertyChanged(ref field, value);
			OnPropertyChanged(nameof(this.SelectedThreadHistory));
			this.ChatLogControl = base.Services!.GetRequiredService<ChatLogUCViewModel>();

			//--Load existing chat
			if (value != null && this.ThreadChatHistory.TryGetValue(value.ThreadId, out ThreadHistory? cachedHistory))
			{
				this.ChatLogControl!.ChatLogMessages = cachedHistory.Messages;
			}
			else
			{
				this.ChatLogControl!.ChatLogMessages = new ObservableCollection<ChatLogMessage>();
			}
        }
	}
	public ThreadHistory? SelectedThreadHistory
	{
		get
		{
			if (this.SelectedThread == null) { return null; }
			ThreadChatHistory.TryGetValue(SelectedThread.ThreadId, out var hist);
			return hist;
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
	public double TokensRatio => (this.SelectedThreadHistory?.TokenCount ?? 0) / (double)(_systemData.MaxTPM == 0 ? 1 : _systemData.MaxTPM);
	public Brush TokenUsageBrush =>
		this.TokensRatio >= 0.8 ? (Brush)App.Current.Resources["ErrorBrush"] :
		this.TokensRatio >= 0.6 ? (Brush)App.Current.Resources["WarningBrush"] :
		(Brush)App.Current.Resources["MainFontForegroundBrush"];
	public int MaxTPM => _systemData.MaxTPM;
	#endregion

	#region "Commands"
	public AsyncCommand OnNewThreadCommand => new AsyncCommand(OnNewThreadCommandAsync);
	public AsyncCommand OnDeleteThreadCommand => new AsyncCommand(OnDeleteThreadCommandAsync);
	public AsyncCommand OnAddMessageCommand => new AsyncCommand(OnAddMessageCommandAsync, () => this.SelectedThread != null);
	public AsyncCommand OnRunCommand => new AsyncCommand(OnRunCommandAsync, () => this.SelectedThread != null);
	public AsyncCommand OnLoadPreviousMessagesCommand => new AsyncCommand(OnLoadPreviousMessagesCommandAsync);
	public AsyncCommand OnSummarizeThreadMessagesCommand => new AsyncCommand(OnSummarizeThreadMessagesCommandAsync);
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

				//--Initialize a chat history for each thread.
				foreach (UserAssistantThread thread in this.Threads)
				{
					//--Sanity condition check.
					if (this.ThreadChatHistory.ContainsKey(thread.ThreadId) == false)
					{
						this.ThreadChatHistory[thread.ThreadId] = new ThreadHistory();
					}
				}
			}
		}
		catch (Exception ex) { await base.OnErrorAsync(ex, "Failed to load assistant threads."); }
		finally { base.IsBusy = false; }
    }
    #endregion

    #region "Private Methods"
	private void UpdateThreadTokenCount(int count)
	{
        if (this.ThreadChatHistory.TryGetValue(this.SelectedThread!.ThreadId, out var threadHistory))
        {
            threadHistory.TokenCount = count;
            OnPropertyChanged(nameof(TokensRatio));
            OnPropertyChanged(nameof(TokenUsageBrush));
        }
    }
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
				this.ThreadChatHistory[newLocalThread.ThreadId] = new ThreadHistory();
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
			ChatLogMessage userMessage = new ChatLogMessage()
			{
				Owner = base.CurrentUser!.FirstName,
				Content = this.UserInput
			};

            this.ChatLogControl!.ChatLogMessages.Add(userMessage);

            await _openAIService.CreateThreadMessageAsync(
				this.SelectedThread!.ThreadId,
				this.UserInput);

            this.UserInput = String.Empty;
			userMessage.IsContentComplete = true;
        }
	}
	private async Task OnRunCommandAsync()
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
				Content = String.Empty,
				IsContentComplete = false
			};
			this.ChatLogControl!.ChatLogMessages.Add(assistantMessage);

			int tokenCount = await _openAIService.RunThreadStreamingAsync(
				this.SelectedThread!.ThreadId,
				this.Assistant!.Id,
				assistantMessage);

			//--Update token count
			UpdateThreadTokenCount(tokenCount);

			assistantMessage.IsContentComplete = true;
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
			StringBuilder sb = new StringBuilder();
			List<ChatLogMessage> previousMessages = await _openAIService.GetPreviousMessagesAsync(this.SelectedThread!.ThreadId);

			//--Fix owners
			foreach (ChatLogMessage msg in previousMessages)
			{
				if (msg.Owner == "User") { msg.Owner = base.CurrentUser!.FirstName; }
				else { msg.Owner = this.Assistant!.Name; }

				//--Append content to 'sb' so we can count tokens.
				sb.Append(msg.Content);
			}

            TiktokenTokenizer tokenizer = TiktokenTokenizer.CreateForModel("gpt-4o");
			int tokenCount = tokenizer.CountTokens(sb.ToString());
			UpdateThreadTokenCount(tokenCount);

			this.ChatLogControl!.ChatLogMessages = new ObservableCollection<ChatLogMessage>(previousMessages);
			await Task.Delay(1000);
			foreach (ChatLogMessage msg in previousMessages)
			{
				//--Mark each message complete after it's been loaded in to the collection so the
				//--attached property can fire.
				msg.IsContentComplete = true;

				//--Small wait in between just in case.
				await Task.Delay(200);
			}
		}
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex);
		}
		finally { base.IsBusy = false; }
	}
	private async Task OnSummarizeThreadMessagesCommandAsync()
	{
		try
		{
            bool result = await base.DialogService!.ShowPromptDialogAsync("Are you sure you want to summarize this thread? The AI will summarize the best it can.");
			if (result)
			{
				this.IsBusy = true;

				int newTokenCount = await _openAIService.SummarizeThreadMessagesAsync(
					this.SelectedThread!.ThreadId,
					this.Assistant!.Id);

				this.ChatLogControl!.ChatLogMessages.Clear();
				await OnLoadPreviousMessagesCommandAsync();
			}
		}
		catch (Exception ex)
		{
			await base.OnErrorAsync(ex);
		}
		finally { base.IsBusy = false; }
	}
    #endregion
}
