using Microsoft.AspNetCore.Mvc;
using Zwoo.Database;

namespace Zwoo.Backend.Shared.Api.Model;

public enum ApiError
{
    None = 0,

    BackendError = 100,
    InvalidCaptcha = 101,
    InvalidClient = 101,

    UnverifiedUser = 110,
    MissingCookie = 111,
    UserNotFound = 112,
    PasswordMismatch = 113,
    SessionIdMismatch = 114,
    InvalidEmail = 115,
    InvalidUsername = 116,
    InvalidPassword = 117,
    InvalidBetaCode = 118,
    InvalidTerms = 119,

    EmailTaken = 120,
    UsernameTaken = 121,
    VerifyFailed = 122,
    AlreadyVerified = 123,

    CantDeleteUser = 135,

    GameNotFound = 140,
    InvalidGameName = 141,
    CantJoinGame = 142,
    NoOpCode = 143,
    OpCodeMissing = 144,
    InvalidGameId = 145,
    AlreadyInGame = 146,
    GameIsFull = 147,
}

public static class ErrorCodeExtensions
{
    public static ProblemDetails ToProblem(this ApiError code, ProblemDetails problem)
    {
        return new ProblemDetails()
        {
            Type = $"https://example.com/some/uri/{(int)code}",
            Title = problem.Title,
            Detail = problem.Detail,
            Status = problem.Status,
            Instance = problem.Instance,
            Extensions = problem.Extensions.Concat(new Dictionary<string, object?>() {
                {"errorCode", (int)code},
                {"errorCodeName", code},
            }).ToDictionary()
        };
    }

    public static ApiError ToApi(this ErrorCode? code)
    {
        return code switch
        {
            ErrorCode.NotVerified => ApiError.UnverifiedUser,
            ErrorCode.UserNotFound => ApiError.UserNotFound,
            ErrorCode.WrongPassword => ApiError.PasswordMismatch,
            ErrorCode.SessionExpired => ApiError.SessionIdMismatch,
            _ => ApiError.BackendError
        };
    }

    public static IActionResult BadRequest(this ControllerBase controller, ApiError code, string title, string detail)
    {
        return controller.BadRequest(code.ToProblem(new ProblemDetails()
        {
            Title = title,
            Detail = detail,
            Instance = controller.HttpContext.Request.Path
        }));
    }

    public static IActionResult NotFound(this ControllerBase controller, ApiError code, string title, string detail)
    {
        return controller.NotFound(code.ToProblem(new ProblemDetails()
        {
            Title = title,
            Detail = detail,
            Instance = controller.HttpContext.Request.Path
        }));
    }

    public static IActionResult Unauthorized(this ControllerBase controller, ApiError code, string title, string detail)
    {
        return controller.Unauthorized(code.ToProblem(new ProblemDetails()
        {
            Title = title,
            Detail = detail,
            Instance = controller.HttpContext.Request.Path
        }));
    }
}