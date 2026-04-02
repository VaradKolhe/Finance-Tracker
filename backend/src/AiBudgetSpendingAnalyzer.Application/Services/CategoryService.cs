using AiBudgetSpendingAnalyzer.Application.Common.Exceptions;
using AiBudgetSpendingAnalyzer.Application.Common.Mappings;
using AiBudgetSpendingAnalyzer.Application.DTOs.Categories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using FluentValidation;

namespace AiBudgetSpendingAnalyzer.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCategoryRequest> _createValidator;
    private readonly IValidator<UpdateCategoryRequest> _updateValidator;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateCategoryRequest> createValidator,
        IValidator<UpdateCategoryRequest> updateValidator)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(int userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAccessibleByUserAsync(userId, isAdmin, cancellationToken);

        return categories
            .OrderByDescending(x => x.IsSystemDefined)
            .ThenBy(x => x.Name)
            .Select(x => x.ToCategoryDto())
            .ToArray();
    }

    public async Task<CategoryDto> CreateCategoryAsync(int userId, bool isAdmin, CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (request.IsSystemDefined && !isAdmin)
        {
            throw new ForbiddenException("Only administrators can create system categories.");
        }

        var category = new Category
        {
            Name = request.Name.Trim(),
            Icon = request.Icon.Trim(),
            ColorHex = request.ColorHex.Trim(),
            IsSystemDefined = request.IsSystemDefined,
            UserId = request.IsSystemDefined ? null : userId
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.ToCategoryDto();
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int userId, bool isAdmin, int categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        EnsureUserCanManageCategory(category, userId, isAdmin);

        category.Name = request.Name.Trim();
        category.Icon = request.Icon.Trim();
        category.ColorHex = request.ColorHex.Trim();
        category.UpdatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.ToCategoryDto();
    }

    public async Task DeleteCategoryAsync(int userId, bool isAdmin, int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        EnsureUserCanManageCategory(category, userId, isAdmin);

        if (await _categoryRepository.IsInUseAsync(categoryId, cancellationToken))
        {
            throw new ConflictException("This category is already used by transactions or budgets and cannot be deleted.");
        }

        _categoryRepository.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureUserCanManageCategory(Category category, int userId, bool isAdmin)
    {
        if (isAdmin)
        {
            return;
        }

        if (category.IsSystemDefined || category.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to modify this category.");
        }
    }
}
