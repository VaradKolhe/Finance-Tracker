using AiBudgetSpendingAnalyzer.Application.DTOs.Categories;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(int userId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateCategoryAsync(int userId, bool isAdmin, CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryDto> UpdateCategoryAsync(int userId, bool isAdmin, int categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(int userId, bool isAdmin, int categoryId, CancellationToken cancellationToken = default);
}
