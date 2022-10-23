namespace ZwooBackend.Controllers.DTO;

public class ChangePassword
{
    public string oldPassword { get; set; } = "";
    
    public string newPassword { get; set; } = "";
}

public class RequestResetPassword
{
    public string email { get; set; } = "";
}

public class ResetPassword
{
    public string code { get; set; } = "";
    public string password { get; set; } = "";
}