namespace AiBudgetSpendingAnalyzer.Application.Common.Exceptions;

public class AppException : Exception
{
    public AppException(string message, int statusCode = 400)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, 404)
    {
    }
}

public sealed class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, 409)
    {
    }
}

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message)
        : base(message, 403)
    {
    }
}

public sealed class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message)
        : base(message, 401)
    {
    }
}
