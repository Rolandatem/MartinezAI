using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Models;
using OpenAI.Assistants;
using System.ClientModel;
using System.Text;
using System.Windows;

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

    public async Task<int> RunThreadStreamingAsync(
        string threadId,
        string assistantId,
        string? lastMessageId,
        ChatLogMessage assistantMessage)
    {
        AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);
        int tokenUsedCount = 0;

        await foreach (StreamingUpdate? update in client.CreateRunStreamingAsync(threadId, assistantId))
        {
            switch (update.UpdateKind)
            {
                case StreamingUpdateReason.Error:
                    throw new Exception($"Error during run: {update}");

                case StreamingUpdateReason.RunFailed:
                    if (update is RunUpdate runFailedUpdate)
                    {
                        throw new Exception($"Error during run: {runFailedUpdate.Value.LastError?.Message}");
                    }
                    break;

                case StreamingUpdateReason.RunRequiresAction:
                    //--TODO
                    break;

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
                            //sb.Append(contentUpdate.Text);
                            assistantMessage.Content += contentUpdate.Text;
                        });
                    }
                    break;

                case StreamingUpdateReason.RunCompleted:
                    if (update is RunUpdate runCompletedUpdate)
                    {
                        tokenUsedCount = runCompletedUpdate.Value.Usage.TotalTokenCount;
                    }
                    break;
                default: break;
            }
        }

        return tokenUsedCount;
    }

    public async Task<List<ChatLogMessage>> GetPreviousMessagesAsync(
        string threadId)
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            MessageCollectionOptions options = new MessageCollectionOptions()
            {
                Order = MessageCollectionOrder.Ascending
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
            //return [.. returnMessages.Skip(Math.Max(0, returnMessages.Count - limit))];
            return returnMessages;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to load previous messages.", ex);
        }
    }

    public async Task<ChatSummarizationResult> SummarizeThreadMessagesAsync(
        string threadId,
        string assistantId,
        int retainLimit = 10)
    {
        try
        {
            AssistantClient client = new AssistantClient(Properties.Settings.Default.OpenAIKey);

            //--Get previous messages
            MessageCollectionOptions options = new MessageCollectionOptions()
            {
                Order = MessageCollectionOrder.Ascending
            };
            List<ChatLogMessage> messageList = new List<ChatLogMessage>();
            await foreach (ThreadMessage threadMessage in client.GetMessagesAsync(
                threadId,
                options))
            {
                messageList.Add(new ChatLogMessage()
                {
                    Owner = threadMessage.Role.ToString() + "|" + threadMessage.Id,
                    Content = threadMessage.Content.FirstOrDefault()?.Text ?? String.Empty
                });
            }

            //--Split messages to where the first contains everything except the last 10
            //--messages and the second should have the remaining.
            int splitIdx = Math.Max(0, messageList.Count - retainLimit);
            if (splitIdx == 0)
            {
                throw new Exception($"Not enough messages to consolidate. There must be more than {retainLimit}.");
            }
            List<ChatLogMessage> consolidationList = messageList.Take(splitIdx).ToList();
            List<ChatLogMessage> retentionList = messageList.Skip(splitIdx).ToList();

            //--Create summary request.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"Please summarize the content of this chat for consolidation. If you do not
                see a message that starts with ""CONTEXT SUMMARY"": your summary should start with that.
                If you do see one then a previous summary was created. Use that as a base for your new
                summary. Do not resummarize this message but extend upon it with any new information you
                think we should keep. Do not change any line items unless you notice conflicting data in
                which case update as necessary. While reviewing the contents below to summarize 
                5 dashes (-----) marks the beginning of a new message. The next line will be
                either 'User' for user message, or 'Assistant' for assistant message. The rest of
                the content will be the message.");

            foreach (ChatLogMessage message in consolidationList)
            {
                sb.AppendLine("-----");
                sb.AppendLine(message.Owner.Split("|")[0]);
                sb.AppendLine(message.Content);
            }

            //--Create new thread to make sure there is enough space to use.
            ClientResult<AssistantThread> newThread = await client.CreateThreadAsync();

            //--Send message with content to summarize using the same assistant for context.
            ClientResult<ThreadMessage> summaryThreadMessage = await client.CreateMessageAsync(
                newThread.Value.Id,
                MessageRole.User,
                [MessageContent.FromText(sb.ToString())]);

            //--Create a use a 'Run' for the summarizing to happen.
            sb = new StringBuilder();
            int newTokenCount = 0;
            await foreach (StreamingUpdate? update in client.CreateRunStreamingAsync(
                newThread.Value.Id,
                assistantId))
            {
                switch (update.UpdateKind)
                {
                    case StreamingUpdateReason.Error:
                        throw new Exception($"Error during consolidation: {update}");

                    case StreamingUpdateReason.RunFailed:
                        if (update is RunUpdate runFailedUpdate)
                        {
                            throw new Exception($"Error during consolidation: {runFailedUpdate.Value.LastError?.Message}");
                        }
                        break;

                    case StreamingUpdateReason.RunCompleted:
                        if (update is RunUpdate runCompleteUpdate)
                        {
                            newTokenCount = runCompleteUpdate.Value.Usage.TotalTokenCount;
                        }
                        break;

                    case StreamingUpdateReason.MessageUpdated:
                        if (update is MessageContentUpdate contentUpdate)
                        {
                            sb.Append(contentUpdate.Text);
                        }
                        break;

                    default: break;
                }
            }

            //--Now that we have the consolidation, we need to delete the existing messages from
            //--the thread.
            List<Task> deleteTasks = new List<Task>();
            foreach (ChatLogMessage toDelete in messageList)
            {
                //ClientResult<MessageDeletionResult> result = await client.DeleteMessageAsync(
                //    threadId,
                //    toDelete.Owner.Split("|")[1]);
                deleteTasks.Add(client.DeleteMessageAsync(
                    threadId,
                    toDelete.Owner.Split("|")[1]));
            }
            await Task.WhenAll(deleteTasks);

            //--Now for context let's add the consolidated message to the thread.
            ClientResult<ThreadMessage> addConsolidatedMessage = await client.CreateMessageAsync(
                threadId,
                MessageRole.Assistant,
                [MessageContent.FromText(sb.ToString())]);

            //--Now loop and enter in retained messages
            string lastMessageId = String.Empty;
            foreach (ChatLogMessage retainedMessage in retentionList)
            {
                ClientResult<ThreadMessage> result = await client.CreateMessageAsync(
                    threadId,
                    (MessageRole)Enum.Parse(typeof(MessageRole), retainedMessage.Owner.Split("|")[0]),
                    [MessageContent.FromText(retainedMessage.Content)]);
                lastMessageId = retainedMessage.Owner.Split("|")[1];
            }

            //--Now delete temp thread for cleanup.
            await client.DeleteThreadAsync(newThread.Value.Id);

            return new ChatSummarizationResult { LastMessageId = lastMessageId, NewTokenCount = newTokenCount };
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to summarize messages.", ex);
        }
    }
}
