using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AiBudgetSpendingAnalyzer.API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User context is missing.");

    protected bool IsAdmin => User.IsInRole("Admin");
}
