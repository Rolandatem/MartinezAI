using MartinezAI.Data.Models;

namespace MartinezAI.WPFApp.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task UpdateUserPasswordAsync(string email, string password);
    Task<User> CreateUserAsync(
        string email,
        string firstName,
        string lastName,
        string preHashPassword,
        bool isAdmin);
    Task ToggleIsAdminAsync(string email);
    Task SaveModifiedUserAsync(User user);
    Task<IQueryable<User>> GetAllUsersAsync();
    Task<UserView> GetAuthenticatedUserAsync(string email);
    Task<string> ResetUserPasswordAsync(string email);
    Task DeleteUserAsync(string email);

    Task<UserAssistantThread> CreateNewThreadAsync(
        string newThreadName,
        string assistantId,
        string threadId);
    Task<IEnumerable<UserAssistantThread>> GetAssistantThreadsAsync(
        string assistantId);
    Task DeleteThreadAsync(
        string threadId);
}
