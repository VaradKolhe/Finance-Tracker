using AiBudgetSpendingAnalyzer.Application.DTOs.Auth;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using AiBudgetSpendingAnalyzer.Application.Services;
using AiBudgetSpendingAnalyzer.Application.Validators;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using FluentAssertions;

namespace AiBudgetSpendingAnalyzer.Application.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnToken()
    {
        var userRepository = new InMemoryUserRepository();
        var service = new AuthService(
            userRepository,
            new FakePasswordHasher(),
            new FakeTokenService(),
            new FakeUnitOfWork(),
            new RegisterRequestValidator(),
            new LoginRequestValidator());

        var response = await service.RegisterAsync(new RegisterRequest(
            "Ava",
            "Patel",
            "ava@example.com",
            "Budget@123",
            5000,
            "Build savings",
            "USD",
            false));

        response.Token.Should().Be("test-token");
        response.User.Email.Should().Be("ava@example.com");
        userRepository.Users.Should().ContainSingle();
        userRepository.Users.Single().PasswordHash.Should().Be("HASH::Budget@123");
    }

    private sealed class InMemoryUserRepository : IUserRepository
    {
        public List<User> Users { get; } = new();

        public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<User?>(Users.FirstOrDefault(x => x.Id == userId));

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult<User?>(Users.FirstOrDefault(x => x.Email == email));

        public Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<User>>(Users);

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            user.Id = Users.Count + 1;
            Users.Add(user);
            return Task.CompletedTask;
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string value) => $"HASH::{value}";

        public bool Verify(string value, string hashedValue) => hashedValue == $"HASH::{value}";
    }

    private sealed class FakeTokenService : ITokenService
    {
        public (string Token, DateTime ExpiresAtUtc) GenerateToken(User user) =>
            ("test-token", DateTime.UtcNow.AddHours(2));
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }
}
