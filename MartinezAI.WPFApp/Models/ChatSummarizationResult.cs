namespace MartinezAI.WPFApp.Models;

public class ChatSummarizationResult
{
    public required string LastMessageId { get; init; }
    public required int NewTokenCount { get; init; }
}
