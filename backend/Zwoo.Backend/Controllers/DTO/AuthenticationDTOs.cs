using System.Text.Json.Serialization;

namespace Zwoo.Backend.Controllers.DTO;

public class CreateAccount
{
    public string username { get; set; } = "";
    public string email { get; set; } = "";
    public string password { get; set; } = "";
    public bool acceptedTerms { get; set; } = false;
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