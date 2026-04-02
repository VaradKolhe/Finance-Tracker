namespace AiBudgetSpendingAnalyzer.Infrastructure.Configuration;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "AiBudgetSpendingAnalyzer";
    public string Audience { get; set; } = "AiBudgetSpendingAnalyzerClient";
    public string Key { get; set; } = "ChangeThisDevelopmentOnlyKey1234567890";
    public int ExpiryMinutes { get; set; } = 120;
}
