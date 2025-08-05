using MartinezAI.Data;
using MartinezAI.Data.Models;
using MartinezAI.WPFApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MartinezAI.WPFApp.Tools;

internal class UserService(
    MartinezAIDbContext _context,
    IHashingService _hashingService) : IUserService
{
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Email == email.Clean());
    }

    public async Task UpdateUserPasswordAsync(
        string email, 
        string password)
    {
        User? user = await GetUserByEmailAsync(email) ?? throw new Exception("User does not exist in database.");
        user.Password = _hashingService.Hash($"{email.Clean()}|{password.Clean()}");
        await _context.SaveChangesAsync();
    }

    public async Task<User> CreateUserAsync(
        string email,
        string firstName,
        string lastName,
        string preHashPassword,
        bool isAdmin)
    {
        User newUser = new User()
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Password = _hashingService.Hash($"{email}|{preHashPassword}"),
            IsAdmin = isAdmin
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }

    public async Task ToggleIsAdminAsync(string email)
    {
        User? user = await GetUserByEmailAsync(email);
        if (user == null) { return; }

        user.IsAdmin = !user.IsAdmin;
        await _context.SaveChangesAsync();
    }

    public async Task SaveModifiedUserAsync(User user)
    {
        _context.Users.Attach(user);
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public Task<IQueryable<User>> GetAllUsersAsync()
    {
        return Task.FromResult(_context.Users.AsQueryable());
    }

    public Task<UserView> GetAuthenticatedUserAsync(string email)
    {
        return _context.View_Users
            .FirstAsync(uv => uv.Email == email);
    }

    public async Task<string> ResetUserPasswordAsync(string email)
    {
        User? user = await this.GetUserByEmailAsync(email) ?? throw new Exception("User does not exist in database.");
        string preHashPassword = _hashingService.GenerateRandomPrehashPassword(20);
        user.Password = _hashingService.Hash($"{user.Email}|{preHashPassword}");
        await _context.SaveChangesAsync();

        return preHashPassword;
    }

    public async Task DeleteUserAsync(string email)
    {
        User? user = await this.GetUserByEmailAsync(email) ?? throw new Exception("User does not exist in database.");
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UserAssistantThread> CreateNewThreadAsync(
        string newThreadName,
        string assistantId,
        string threadId)
    {
        UserAssistantThread newThread = new UserAssistantThread()
        {
            ThreadName = newThreadName,
            AssistantId = assistantId,
            ThreadId = threadId
        };

        _context.UserAssistantThreads.Add(newThread);
        await _context.SaveChangesAsync();

        return newThread;
    }

    public async Task<IEnumerable<UserAssistantThread>> GetAssistantThreadsAsync(
        string assistantId)
    {
        return await _context.UserAssistantThreads
            .Where(thread => thread.AssistantId == assistantId)
            .ToListAsync();
    }

    public async Task DeleteThreadAsync(
        string threadId)
    {
        UserAssistantThread? thread = await _context.UserAssistantThreads
            .FirstOrDefaultAsync(thread => thread.ThreadId == threadId);

        if (thread == null) { throw new Exception("Thread does not exist in database."); }

        _context.UserAssistantThreads.Remove(thread);
        await _context.SaveChangesAsync();
    }
}