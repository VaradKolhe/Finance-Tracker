using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

namespace AiBudgetSpendingAnalyzer.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string value) => BCrypt.Net.BCrypt.HashPassword(value);

    public bool Verify(string value, string hashedValue) => BCrypt.Net.BCrypt.Verify(value, hashedValue);
}
