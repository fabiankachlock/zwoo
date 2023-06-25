namespace ZwooBackend.Controllers.DTO;

public class SetSettings
{
    public string settings { get; set; } = "";
}

public class ChangePassword
{
    public string oldPassword { get; set; } = "";

    public string newPassword { get; set; } = "";
}

public class RequestResetPassword
{
    public string email { get; set; } = "";
    public string captchaToken { get; set; } = "";

}

public class ResetPassword
{
    public string code { get; set; } = "";
    public string password { get; set; } = "";
    public string captchaToken { get; set; } = "";

}