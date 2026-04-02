using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AiBudgetSpendingAnalyzer.Infrastructure.Data;

public static class AppDbContextSeed
{
    public static async Task InitializeAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (!await dbContext.Categories.AnyAsync(x => x.IsSystemDefined, cancellationToken))
        {
            await dbContext.Categories.AddRangeAsync(GetDefaultCategories(), cancellationToken);
        }

        if (!await dbContext.Users.AnyAsync(x => x.Role == UserRole.Admin, cancellationToken))
        {
            await dbContext.Users.AddAsync(new User
            {
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@budgetai.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@12345"),
                Role = UserRole.Admin,
                CurrencyCode = "USD",
                FinancialGoal = "Maintain healthy cash flow and oversee the system.",
                MonthlyIncome = 0
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Category> GetDefaultCategories()
    {
        return new[]
        {
            new Category { Name = "Salary", Icon = "payments", ColorHex = "#2563EB", IsSystemDefined = true },
            new Category { Name = "Food", Icon = "restaurant", ColorHex = "#F97316", IsSystemDefined = true },
            new Category { Name = "Housing", Icon = "home", ColorHex = "#059669", IsSystemDefined = true },
            new Category { Name = "Transport", Icon = "directions_car", ColorHex = "#8B5CF6", IsSystemDefined = true },
            new Category { Name = "Utilities", Icon = "bolt", ColorHex = "#14B8A6", IsSystemDefined = true },
            new Category { Name = "Entertainment", Icon = "movie", ColorHex = "#EC4899", IsSystemDefined = true },
            new Category { Name = "Healthcare", Icon = "favorite", ColorHex = "#EF4444", IsSystemDefined = true },
            new Category { Name = "Savings", Icon = "savings", ColorHex = "#22C55E", IsSystemDefined = true },
            new Category { Name = "Shopping", Icon = "shopping_bag", ColorHex = "#0EA5E9", IsSystemDefined = true },
            new Category { Name = "Travel", Icon = "flight", ColorHex = "#A855F7", IsSystemDefined = true }
        };
    }
}
