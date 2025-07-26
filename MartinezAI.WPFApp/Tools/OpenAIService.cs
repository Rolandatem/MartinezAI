using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Models;
using OpenAI.Assistants;
using System.ClientModel;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MartinezAI.WPFApp.Tools;

internal class OpenAIService : IOpenAIService
{
    public async Task<IEnumerable<Assistant>> GetAssistantsAsync()
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            List<Assistant> returnList = new List<Assistant>();
            await foreach (Assistant assistant in client.GetAssistantsAsync())
            {
                returnList.Add(assistant);
            }
            
            return returnList;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve assistants.", ex);
        }
    }

    public async Task<Assistant> CreateAssistantAsync(
        AssistantCreationOptions options,
        string model = "gpt-4o")
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            ClientResult<Assistant> result = await client.CreateAssistantAsync(model, options);

            return result.Value;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to create the assistant.", ex);
        }
    }

    public async Task<Assistant> UpdateAssistantAsync(
        string assistantId,
        AssistantModificationOptions options)
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            ClientResult<Assistant> result = await client.ModifyAssistantAsync(assistantId, options);

            return result.Value;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to update the assistant.", ex);
        }
    }

    public async Task<bool> DeleteAssistantAsync(
        string assistantId)
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            ClientResult<AssistantDeletionResult> result = await client.DeleteAssistantAsync(assistantId);

            return result.Value.Deleted;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to delete assistant.", ex);
        }
    }

    public async Task<AssistantThread> CreateNewThreadAsync()
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            ClientResult<AssistantThread> result = await client.CreateThreadAsync();

            return result.Value;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to create thread.", ex);
        }
    }

    public async Task DeleteThreadAsync(
        string threadId)
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            ClientResult<ThreadDeletionResult> result = await client.DeleteThreadAsync(threadId);

            if (result.Value.Deleted == false)
            {
                throw new Exception("OpenAI declined deletion.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to delete thread.", ex);
        }
    }

    public async Task CreateThreadMessageAsync(
        string threadId,
        string message)
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            ClientResult<ThreadMessage> result = await client.CreateMessageAsync(
                threadId,
                MessageRole.User,
                new List<MessageContent>() { MessageContent.FromText(message) });
        }
        catch(Exception ex)
        {
            throw new Exception("Failed to create message.", ex);
        }
    }

    public async Task<(List<string> messages, string lastMessageId)> RunThreadAsync(
        string threadId,
        string assistantId,
        string? lastMessageId)
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            ClientResult<ThreadRun> createRunResult = await client.CreateRunAsync(
                threadId,
                assistantId);

            //--Poll until a terminal status is found.
            ClientResult<ThreadRun>? checkRun = null;
            do
            {
                await Task.Delay(500);

                checkRun = await client.GetRunAsync(threadId, createRunResult.Value.Id);
            } while (checkRun.Value.Status.IsTerminal == false);

            //--Requires action status. For function calling, implement later.
            if (checkRun.Value.Status == RunStatus.RequiresAction)
            {
                throw new NotImplementedException("Function call requested.");
            }

            //--Canceled status
            if (checkRun.Value.Status == RunStatus.Cancelled) { throw new Exception("Assistant AI canceled run."); }

            //--Failed status
            if (checkRun.Value.Status == RunStatus.Failed) { throw new Exception($"Assistant AI failed run. {Environment.NewLine}{checkRun.Value.LastError?.Message}"); }

            //--Success status
            List<string> assistantMessages = new List<string>();
            string newLastMessageId = String.Empty;
            DateTimeOffset maxTimestamp = DateTimeOffset.MinValue;
            MessageCollectionOptions options = new MessageCollectionOptions();
            options.Order = MessageCollectionOrder.Ascending;
            if (lastMessageId!.Exists()) { options.AfterId = lastMessageId; }
            if (checkRun.Value.Status == RunStatus.Completed)
            {
                await foreach (ThreadMessage threadMessage in client.GetMessagesAsync(threadId, options))
                {
                    if (maxTimestamp < threadMessage.CreatedAt)
                    {
                        maxTimestamp = threadMessage.CreatedAt;
                        newLastMessageId = threadMessage.Id;
                    }

                    if (threadMessage.Role == MessageRole.Assistant)
                    {
                        foreach (MessageContent contentItem in threadMessage.Content)
                        {
                            if (contentItem.Text.Exists())
                            {
                                assistantMessages.Add(contentItem.Text);
                            }
                        }
                    }
                }
            }

            return (assistantMessages, newLastMessageId);
        }
        catch(Exception ex)
        {
            throw new Exception("Failed to run message.", ex);
        }
    }

    public async Task RunThreadStreamingAsync(
        string threadId,
        string assistantId,
        string? lastMessageId,
        ChatLogMessage assistantMessage)
    {
        AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

        await foreach (StreamingUpdate? update in client.CreateRunStreamingAsync(threadId, assistantId))
        {
            switch (update.UpdateKind)
            {
                case StreamingUpdateReason.Error:
                    throw new Exception($"Error during run: {update}");

                case StreamingUpdateReason.MessageCreated:
                    //--New message started, get ID for tracking
                    if (update is MessageStatusUpdate messageUpdate)
                    {
                        lastMessageId = messageUpdate.Value.Id;
                    }
                    break; //--Check type to case, hopefully we can get the message id from this.

                case StreamingUpdateReason.MessageUpdated:
                    if (update is MessageContentUpdate contentUpdate)
                    {
                        //--Make sure to run on UI thread.
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            assistantMessage.Content += contentUpdate.Text;
                        });
                    }
                    break;
                default: break;
            }


            if (update.UpdateKind == StreamingUpdateReason.Error)
            {
                throw new Exception($"Error during run: {update}");
            }
            else if (update.UpdateKind == StreamingUpdateReason.RunFailed)
            {
                if (update is RunUpdate runUpdate)
                {
                    throw new Exception($"Error during run: {runUpdate.Value.LastError?.Message}");
                }
            }
            else if (update.UpdateKind == StreamingUpdateReason.RunRequiresAction)
            {
                //--include tool somehow.
            }
            else if (update.UpdateKind == StreamingUpdateReason.MessageUpdated)
            {
                if (update is MessageContentUpdate contentUpdate)
                {
                    //--Do something with contentUpdate.Text
                }
                else { throw new Exception("different content"); }
            }
        }
    }

    public async Task<List<ChatLogMessage>> GetPreviousMessagesAsync(
        string threadId,
        int limit = 10)
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            MessageCollectionOptions options = new MessageCollectionOptions()
            {
                Order = MessageCollectionOrder.Ascending,
                PageSizeLimit = limit
            };

            List<ChatLogMessage> returnMessages = new List<ChatLogMessage>();
            await foreach (ThreadMessage threadMessage in client.GetMessagesAsync(
                threadId,
                options))
            {
                foreach (MessageContent content in threadMessage.Content)
                {
                    returnMessages.Add(new ChatLogMessage()
                    {
                        Owner = threadMessage.Role.ToString(),
                        Content = content.Text
                    });
                }
            }

            //--Currently there is a bug and the PageSizeLimit is not being properly used. In the meantime
            //--we will just return the last of limit.
            return [.. returnMessages.Skip(Math.Max(0, returnMessages.Count - limit))];
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to load previous messages.", ex);
        }
    }
}
