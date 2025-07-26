namespace MartinezAI.Data.Models;

public class UserView
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required bool IsAdmin { get; set; }
}
