namespace AiBudgetSpendingAnalyzer.Infrastructure.Configuration;

public class OpenAiSettings
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-5.4-mini";
    public string Endpoint { get; set; } = "https://api.openai.com/v1/responses";
}
