using AiBudgetSpendingAnalyzer.Application.DTOs.Categories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiBudgetSpendingAnalyzer.API.Controllers;

[Authorize]
[Route("api/categories")]
public class CategoriesController : BaseApiController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CategoryDto>>> Get(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetCategoriesAsync(CurrentUserId, IsAdmin, cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.CreateCategoryAsync(CurrentUserId, IsAdmin, request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
    }

    [HttpPut("{categoryId:int}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoryDto>> Update(int categoryId, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.UpdateCategoryAsync(CurrentUserId, IsAdmin, categoryId, request, cancellationToken);
        return Ok(category);
    }

    [HttpDelete("{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int categoryId, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteCategoryAsync(CurrentUserId, IsAdmin, categoryId, cancellationToken);
        return NoContent();
    }
}
