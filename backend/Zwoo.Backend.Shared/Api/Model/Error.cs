using Microsoft.AspNetCore.Mvc;

namespace Zwoo.Backend.Shared.Api.Model;

public enum ErrorCode
{
    Example = 0
}

public static class ErrorCodeExtensions
{
    public static ProblemDetails ToProblem(this ErrorCode code, ProblemDetails problem)
    {
        return new ProblemDetails()
        {
            Type = $"https://example.com/some/uri/{(int)code}",
            Title = problem.Title,
            Detail = problem.Detail,
            Status = problem.Status,
            Instance = problem.Instance,
            Extensions = problem.Extensions.Concat(new Dictionary<string, object?>() {
                {"code", (int)code},
                {"humanCode", code},
            }).ToDictionary()
        };
    }
}