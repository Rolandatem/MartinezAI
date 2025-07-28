using MartinezAI.WPFApp.Models;
using OpenAI.Assistants;

namespace MartinezAI.WPFApp.Interfaces;

public interface IOpenAIService
{
    Task<IEnumerable<Assistant>> GetAssistantsAsync();
    Task<Assistant> CreateAssistantAsync(
        AssistantCreationOptions options,
        string model = "gpt-4o");
    Task<Assistant> UpdateAssistantAsync(
        string assitantId,
        AssistantModificationOptions options);
    Task<bool> DeleteAssistantAsync(string assitantId);
    Task<AssistantThread> CreateNewThreadAsync();
    Task DeleteThreadAsync(string threadId);
    Task CreateThreadMessageAsync(
        string threadId,
        string message);
    Task<(List<string> messages, string lastMessageId)> RunThreadAsync(
            string threadId,
            string assistantId,
            string? lastMessageid);
    Task<int> RunThreadStreamingAsync(
        string threadId,
        string assistantId,
        string? lastMessageid,
        ChatLogMessage assistantMessage);
    Task<List<ChatLogMessage>> GetPreviousMessagesAsync(
        string threadId);
    Task<ChatSummarizationResult> SummarizeThreadMessagesAsync(
        string threadId,
        string assistantId,
        int retainLimit = 10);
}