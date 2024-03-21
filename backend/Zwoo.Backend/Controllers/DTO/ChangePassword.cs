namespace Zwoo.Backend.Controllers.DTO;

public class ChangePassword
{
    public string OldPassword { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;
}