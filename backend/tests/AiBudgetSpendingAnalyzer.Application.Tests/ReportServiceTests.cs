using AiBudgetSpendingAnalyzer.Application.DTOs.Reports;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Services;
using AiBudgetSpendingAnalyzer.Application.Validators;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Domain.Enums;
using FluentAssertions;

namespace AiBudgetSpendingAnalyzer.Application.Tests;

public class ReportServiceTests
{
    [Fact]
    public async Task GenerateAsync_ShouldHighlightLargestExpenseCategory()
    {
        var foodCategory = new Category { Id = 1, Name = "Food", ColorHex = "#f97316", Icon = "restaurant" };
        var salaryCategory = new Category { Id = 2, Name = "Salary", ColorHex = "#0f766e", Icon = "payments" };

        var transactionRepository = new FakeTransactionRepository(
            new[]
            {
            new Transaction
            {
                Id = 1,
                UserId = 7,
                Amount = 4000,
                Type = TransactionType.Income,
                CategoryId = salaryCategory.Id,
                Category = salaryCategory,
                TransactionDateUtc = DateTime.UtcNow.AddDays(-3)
            },
            new Transaction
            {
                Id = 2,
                UserId = 7,
                Amount = 900,
                Type = TransactionType.Expense,
                CategoryId = foodCategory.Id,
                Category = foodCategory,
                TransactionDateUtc = DateTime.UtcNow.AddDays(-2)
            },
            new Transaction
            {
                Id = 3,
                UserId = 7,
                Amount = 250,
                Type = TransactionType.Expense,
                CategoryId = foodCategory.Id,
                Category = foodCategory,
                TransactionDateUtc = DateTime.UtcNow.AddDays(-1)
            }
            });

        var reportService = new ReportService(
            transactionRepository,
            new FakeBudgetRepository(
                new[]
                {
                new Budget
                {
                    Id = 1,
                    UserId = 7,
                    CategoryId = foodCategory.Id,
                    Category = foodCategory,
                    LimitAmount = 1000,
                    Month = DateTime.UtcNow.Month,
                    Year = DateTime.UtcNow.Year
                }
                }),
            new ReportRequestDtoValidator());

        var report = await reportService.GenerateAsync(7, new ReportRequestDto("weekly", null, null));

        report.Recommendations.Should().ContainSingle(x => x.Contains("Food accounts for", StringComparison.OrdinalIgnoreCase));
        report.Metrics.Should().Contain(x => x.Label == "Expenses" && x.Value == 1150);
    }

    private sealed class FakeTransactionRepository : ITransactionRepository
    {
        private readonly IReadOnlyCollection<Transaction> _transactions;

        public FakeTransactionRepository(IReadOnlyCollection<Transaction> transactions)
        {
            _transactions = transactions;
        }

        public Task<Transaction?> GetByIdAsync(int transactionId, CancellationToken cancellationToken = default) =>
            Task.FromResult<Transaction?>(_transactions.FirstOrDefault(x => x.Id == transactionId));

        public Task<IReadOnlyCollection<Transaction>> GetByUserAsync(int userId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken cancellationToken = default)
        {
            var query = _transactions.Where(x => x.UserId == userId);

            if (fromUtc.HasValue)
            {
                query = query.Where(x => x.TransactionDateUtc >= fromUtc.Value);
            }

            if (toUtc.HasValue)
            {
                query = query.Where(x => x.TransactionDateUtc < toUtc.Value);
            }

            return Task.FromResult<IReadOnlyCollection<Transaction>>(query.ToArray());
        }

        public Task<IReadOnlyCollection<Transaction>> GetRecentAsync(int userId, int count, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<Transaction>>(_transactions.Where(x => x.UserId == userId).Take(count).ToArray());

        public Task<IReadOnlyCollection<Transaction>> GetAllAsync(DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken cancellationToken = default) =>
            Task.FromResult(_transactions);

        public Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(Transaction transaction)
        {
        }
    }

    private sealed class FakeBudgetRepository : IBudgetRepository
    {
        private readonly IReadOnlyCollection<Budget> _budgets;

        public FakeBudgetRepository(IReadOnlyCollection<Budget> budgets)
        {
            _budgets = budgets;
        }

        public Task<Budget?> GetByIdAsync(int budgetId, CancellationToken cancellationToken = default) =>
            Task.FromResult<Budget?>(_budgets.FirstOrDefault(x => x.Id == budgetId));

        public Task<Budget?> GetByCategoryAndPeriodAsync(int userId, int categoryId, int year, int month, CancellationToken cancellationToken = default) =>
            Task.FromResult<Budget?>(_budgets.FirstOrDefault(x => x.UserId == userId && x.CategoryId == categoryId && x.Year == year && x.Month == month));

        public Task<IReadOnlyCollection<Budget>> GetByUserAsync(int userId, int? year = null, int? month = null, CancellationToken cancellationToken = default)
        {
            var query = _budgets.Where(x => x.UserId == userId);

            if (year.HasValue)
            {
                query = query.Where(x => x.Year == year.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(x => x.Month == month.Value);
            }

            return Task.FromResult<IReadOnlyCollection<Budget>>(query.ToArray());
        }

        public Task AddAsync(Budget budget, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(Budget budget)
        {
        }
    }
}
