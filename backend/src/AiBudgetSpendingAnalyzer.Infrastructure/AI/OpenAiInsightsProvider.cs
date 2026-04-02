using AiBudgetSpendingAnalyzer.Application.DTOs.AI;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Domain.Enums;
using AiBudgetSpendingAnalyzer.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AiBudgetSpendingAnalyzer.Infrastructure.AI;

public class OpenAiInsightsProvider : IAiInsightsProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _settings;
    private readonly ILogger<OpenAiInsightsProvider> _logger;

    public OpenAiInsightsProvider(
        HttpClient httpClient,
        IOptions<OpenAiSettings> options,
        ILogger<OpenAiInsightsProvider> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<AiAnalysisDto> GenerateAsync(User user, IReadOnlyCollection<Transaction> transactions, CancellationToken cancellationToken = default)
    {
        if (!transactions.Any())
        {
            return new AiAnalysisDto(
                "There are no transactions in the last 30 days yet, so the analyzer does not have enough data to identify patterns.",
                new[] { "Add a few income and expense transactions to unlock personalized insights." },
                new[] { "Track at least one full week of spending for more meaningful recommendations." });
        }

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            return BuildMockAnalysis(user, transactions);
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _settings.Endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(BuildRequestPayload(user, transactions)),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("OpenAI analysis failed with status {StatusCode}: {Body}", response.StatusCode, responseBody);
                return BuildMockAnalysis(user, transactions);
            }

            using var document = JsonDocument.Parse(responseBody);
            var outputText = TryExtractOutputText(document.RootElement);
            if (string.IsNullOrWhiteSpace(outputText))
            {
                return BuildMockAnalysis(user, transactions);
            }

            var structured = JsonSerializer.Deserialize<StructuredAiResponse>(outputText);
            if (structured is null || string.IsNullOrWhiteSpace(structured.Summary))
            {
                return BuildMockAnalysis(user, transactions);
            }

            return new AiAnalysisDto(
                structured.Summary,
                structured.Insights?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray() ?? Array.Empty<string>(),
                structured.Recommendations?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray() ?? Array.Empty<string>());
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Falling back to mock AI analysis.");
            return BuildMockAnalysis(user, transactions);
        }
    }

    private object BuildRequestPayload(User user, IReadOnlyCollection<Transaction> transactions)
    {
        return new
        {
            model = _settings.Model,
            instructions = "You are a personal finance analyst. Return a concise JSON object with a summary, spending insights, and practical saving recommendations.",
            input = BuildPrompt(user, transactions),
            store = false,
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "spending_analysis",
                    strict = true,
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            summary = new { type = "string" },
                            insights = new
                            {
                                type = "array",
                                items = new { type = "string" }
                            },
                            recommendations = new
                            {
                                type = "array",
                                items = new { type = "string" }
                            }
                        },
                        required = new[] { "summary", "insights", "recommendations" },
                        additionalProperties = false
                    }
                }
            }
        };
    }

    private static string BuildPrompt(User user, IReadOnlyCollection<Transaction> transactions)
    {
        var lines = transactions
            .OrderByDescending(x => x.TransactionDateUtc)
            .Select(x => $"{x.TransactionDateUtc:yyyy-MM-dd} | {x.Type} | {x.Category.Name} | {FormatAmount(x.Amount, user.CurrencyCode)} | {x.Notes}");

        return $"""
Analyze this user's last 30 days of budget activity and provide actionable spending feedback.

User profile:
- Monthly income: {FormatAmount(user.MonthlyIncome, user.CurrencyCode)}
- Financial goal: {user.FinancialGoal}
- Currency: {user.CurrencyCode}

Transactions:
{string.Join(Environment.NewLine, lines)}

Return:
1. A short spending summary.
2. 3-5 insights about habits or concentration of spending.
3. 3-5 recommendations that help the user save money.
""";
    }

    private static string? TryExtractOutputText(JsonElement root)
    {
        if (root.TryGetProperty("output_text", out var outputTextElement) && outputTextElement.ValueKind == JsonValueKind.String)
        {
            return outputTextElement.GetString();
        }

        if (!root.TryGetProperty("output", out var outputElement) || outputElement.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var outputItem in outputElement.EnumerateArray())
        {
            if (!outputItem.TryGetProperty("content", out var contentElement) || contentElement.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var contentItem in contentElement.EnumerateArray())
            {
                if (contentItem.TryGetProperty("text", out var textElement) && textElement.ValueKind == JsonValueKind.String)
                {
                    return textElement.GetString();
                }
            }
        }

        return null;
    }

    private static AiAnalysisDto BuildMockAnalysis(User user, IReadOnlyCollection<Transaction> transactions)
    {
        var expenses = transactions.Where(x => x.Type == TransactionType.Expense).ToArray();
        var income = transactions.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount);
        var totalExpenses = expenses.Sum(x => x.Amount);
        var topCategories = expenses
            .GroupBy(x => x.Category.Name)
            .Select(group => new { Category = group.Key, Amount = group.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Amount)
            .Take(3)
            .ToArray();

        var summaryParts = new List<string>
        {
            $"In the last 30 days, you recorded {transactions.Count} transactions."
        };

        if (income > 0)
        {
            summaryParts.Add($"Income totaled {FormatAmount(income, user.CurrencyCode)}.");
        }

        summaryParts.Add($"Expenses totaled {FormatAmount(totalExpenses, user.CurrencyCode)}.");

        var insights = new List<string>();
        var recommendations = new List<string>();

        foreach (var category in topCategories)
        {
            var share = totalExpenses == 0 ? 0 : Math.Round((category.Amount / totalExpenses) * 100, 2);
            insights.Add($"{category.Category} accounts for {share}% of your recent expenses.");

            if (share >= 35)
            {
                recommendations.Add($"Review {category.Category} spending first, because it is your largest opportunity for savings.");
            }

            if (category.Category.Contains("food", StringComparison.OrdinalIgnoreCase) ||
                category.Category.Contains("restaurant", StringComparison.OrdinalIgnoreCase))
            {
                recommendations.Add("You are spending heavily on food-related purchases. Consider setting a weekly dining cap.");
            }
        }

        if (income > 0)
        {
            var expenseRatio = Math.Round((totalExpenses / income) * 100, 2);
            insights.Add($"You spent about {expenseRatio}% of your recent income.");

            if (expenseRatio > 85)
            {
                recommendations.Add("Your spending is close to your income. Try trimming one discretionary category this month to improve savings.");
            }
        }

        if (!recommendations.Any())
        {
            recommendations.Add("Your spending mix looks stable. Keep tracking daily transactions so you can spot shifts early.");
        }

        if (!insights.Any())
        {
            insights.Add("Most of your recent activity is balanced, with no single category dominating the period.");
        }

        return new AiAnalysisDto(
            string.Join(" ", summaryParts),
            insights.Distinct().Take(5).ToArray(),
            recommendations.Distinct().Take(5).ToArray());
    }

    private static string FormatAmount(decimal amount, string currencyCode) =>
        $"{amount.ToString("0.00", CultureInfo.InvariantCulture)} {currencyCode}";

    private sealed class StructuredAiResponse
    {
        public string Summary { get; set; } = string.Empty;
        public string[] Insights { get; set; } = Array.Empty<string>();
        public string[] Recommendations { get; set; } = Array.Empty<string>();
    }
}
