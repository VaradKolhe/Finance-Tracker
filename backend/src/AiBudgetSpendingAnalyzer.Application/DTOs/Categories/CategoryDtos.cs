namespace AiBudgetSpendingAnalyzer.Application.DTOs.Categories;

public record CategoryDto(
    int Id,
    string Name,
    string Icon,
    string ColorHex,
    bool IsSystemDefined,
    int? UserId);

public record CreateCategoryRequest(
    string Name,
    string Icon,
    string ColorHex,
    bool IsSystemDefined);

public record UpdateCategoryRequest(
    string Name,
    string Icon,
    string ColorHex);
