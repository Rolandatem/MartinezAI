namespace MartinezAI.Data.Models;

public class UserAssistantThread
{
    public int Id { get; set; }
    public required string ThreadName { get; set; }
    public required string AssistantId { get; set; }
    public required string ThreadId { get; set; }
    public string? LastMessageId { get; set; }
}
