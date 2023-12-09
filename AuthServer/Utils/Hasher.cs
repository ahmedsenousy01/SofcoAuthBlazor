using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Utils;
public static class Hasher
{
    public static string HashPassword(string password)
    {
        return new PasswordHasher<object>().HashPassword(null!, password);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var passwordVerificationResult = new PasswordHasher<object>().VerifyHashedPassword(null!, hashedPassword, password);
        return passwordVerificationResult == PasswordVerificationResult.Success;
    }

    public static string BasicSha256Hash(string password)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }

    public static bool VerifySha256Hash(string plain, string hash)
    {
        string hashedInput = BasicSha256Hash(plain);
        return hashedInput.Equals(hash, StringComparison.OrdinalIgnoreCase);
    }
}