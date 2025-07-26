using MartinezAI.WPFApp.Interfaces;
using System.Security.Cryptography;

namespace MartinezAI.WPFApp.Tools;

internal class HashingService : IHashingService
{
    private const int SaltSize = 16;   // 128 bits
    private const int KeySize = 32;    // 256 bits
    private const int Iterations = 100_000;

    public string Hash(string input)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        using var pbkdf2 = new Rfc2898DeriveBytes(input, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] key = pbkdf2.GetBytes(KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string input, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 3) return false;

        int iterations = int.Parse(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] key = Convert.FromBase64String(parts[2]);

        using var pbkdf2 = new Rfc2898DeriveBytes(input, salt, iterations, HashAlgorithmName.SHA256);
        byte[] computedKey = pbkdf2.GetBytes(KeySize);

        return CryptographicOperations.FixedTimeEquals(key, computedKey);
    }

    public string GenerateRandomPrehashPassword(int length)
    {
        char[] PasswordChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?".ToCharArray();
        byte[] byteArray = RandomNumberGenerator.GetBytes(length);
        string pw = new String([.. byteArray.Select(b => PasswordChars[b % PasswordChars.Length])]);

        return pw;
    }
}
