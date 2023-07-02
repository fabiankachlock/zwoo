using System.Text.Json.Serialization;

namespace ZwooBackend.Controllers.DTO;

public class CreateAccount
{
    public string username { get; set; } = "";
    public string email { get; set; } = "";
    public string password { get; set; } = "";
    public string? code { get; set; } = "";
    public string captchaToken { get; set; } = "";

}

public class Login
{
    public string email { get; set; } = "";
    public string password { get; set; } = "";
    public string captchaToken { get; set; } = "";
}

public class Delete
{
    public string password { get; set; } = "";
}

public class VerificationEmail
{
    public string email { get; set; } = "";
}