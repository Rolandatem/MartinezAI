namespace MartinezAI.WPFApp.Interfaces;

public interface IHashingService
{
    string Hash(string input);
    bool Verify(string input, string storedHash);
    string GenerateRandomPrehashPassword(int length);
}
